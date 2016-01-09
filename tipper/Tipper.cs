using System;
using System.Collections.Generic;
using System.Linq;
using ArtificialNeuralNetwork;
using AustralianRulesFootball;
using Utilities;

namespace Tipper
{
    public class Tipper
    {
        public static int NumInputs = 18;
        public static int NumOutputs = 4;
        public static int DefaultHiddens = 5;
        public static int RelevantRoundHistory = 14;
        public List<Team> Teams;
        public League League;
        public Network Net;

        public Tipper()
        {
            Teams = Util.GetTeams();
            League = League.Load();
            Refresh(NumInputs, new List<int>() {DefaultHiddens}, NumOutputs);
        }

        public void Refresh(int inputs, List<int> hiddens, int outputs)
        {
            Net = new Network(inputs, hiddens, outputs);
        }

        public Data LearnFromTo(int fromYear, int fromRound, DateTime date)
        {
            Refresh(NumInputs, new List<int>() {DefaultHiddens}, NumOutputs);
            var round = GetRoundFromDate(date);
            return LearnFromTo(fromYear, fromRound, date.Year, round.Number);
        }

        private Round GetRoundFromDate(DateTime date)
        {
            var seasons = League.Seasons;
            var rounds = new List<Round>();
            var matches = new List<Match>();
            foreach (var s in seasons)
            {
                rounds.AddRange(s.Rounds);
            }
            foreach (var r in rounds)
            {
                matches.AddRange(r.Matches);
            }
            var match = matches.OrderByDescending(m => m.Date).First(m => m.Date < date);
            var round = rounds.First(r => r.Matches.Any(m => m.Equals(match)));
            return round;
        }

        public Data LearnFromTo(int fromYear, int fromRound, int toYear, int toRound, int minMargin = 0,
            int minTotalScore = 0)
        {
            var data = new Data();
            var rounds = League.GetRounds(0, 0, toYear, toRound).Where(x => x.Matches.Count > 0).ToList();
            foreach (
                var m in
                    rounds.Where(r => (r.Year == fromYear && r.Number >= fromRound) || (r.Year > fromYear))
                        .SelectMany(r => r.Matches))
                //.Where(m => m.Margin() >= minMargin && m.TotalScore() >= minTotalScore))
            {
                var datapoint = new DataPoint();
                var season = new Season(toYear, rounds.Where(r => !r.Matches.Any(rm => rm.Date >= m.Date)).ToList());
                datapoint.Inputs = (BuildInputs(season, m));
                datapoint.Outputs = (new List<double>()
                {
                    Numbery.Normalise(m.HomeScore().Goals, Util.MaxGoals),
                    Numbery.Normalise(m.HomeScore().Points, Util.MaxPoints),
                    Numbery.Normalise(m.AwayScore().Goals, Util.MaxGoals),
                    Numbery.Normalise(m.AwayScore().Points, Util.MaxPoints),
                });
                datapoint.Reference = (m.Home.ApiName + " Vs " + m.Away.ApiName);
                data.DataPoints.Add(datapoint);
            }
            return data;
        }

        public List<Match> PredictNext(DateTime date, bool print)
        {
            var round = GetRoundFromDate(date);
            return Predict(date.Year, round.Number + 1, print);
        }

        public List<Match> Predict(RoundShell round, bool print)
        {
            return Predict(round.Year, round.Number, print);
        }

        public List<Match> Predict(int year, int round, bool print)
        {
            var results = new List<Match>();
            var rounds = League.GetRounds(0, 0, year, round).Where(x => x.Matches.Count > 0).ToList();

            foreach (var m in rounds.Where(r => (r.Year == year && r.Number == round)).SelectMany(r => r.Matches))
            {
                var s = new Season(year, rounds.Where(r => !r.Matches.Any(rm => rm.Date >= m.Date)).ToList());
                var test = BuildInputs(s, m);

                var result = Net.Run(test);
                results.Add(new Match(
                    m.Home,
                    new Score(0, 0),
                    new Score(0, 0),
                    new Score(0, 0),
                    new Score(
                        Numbery.Denormalise(result[0], Util.MaxGoals),
                        Numbery.Denormalise(result[1], Util.MaxPoints)
                        ),
                    m.Away,
                    new Score(0, 0),
                    new Score(0, 0),
                    new Score(0, 0),
                    new Score(
                        Numbery.Denormalise(result[2], Util.MaxGoals),
                        Numbery.Denormalise(result[3], Util.MaxPoints)
                        ),
                    m.Ground, m.Date));

                if (print)
                    Console.WriteLine(m.Home.Mascot + " Vs " + m.Away.Mascot + ": " +
                                      Printlayer(new[]
                                      {
                                          results.Last().HomeScore().Goals,
                                          results.Last().HomeScore().Points,
                                          results.Last().HomeScore().Total(),
                                          results.Last().AwayScore().Goals,
                                          results.Last().AwayScore().Points,
                                          results.Last().AwayScore().Total()
                                      }));
            }
            return results;
        }



        private static IEnumerable<double> ExtractTeamScoreInputSet(Match m, List<Match> matches, int term)
        {
            Func<Match, bool> homeWherePredicate = (x => x.HasTeam(m.Home));
            Func<Match, bool> awayWherePredicate = (x => x.HasTeam(m.Away));
            return ExtractInputSet(m, matches, term, homeWherePredicate, awayWherePredicate);
        }

        private static IEnumerable<double> ExtractGroundScoreInputSet(Match m, List<Match> matches, int term)
        {
            const int relevantYearsDifference = -12;

            Func<Match, bool> homeWherePredicate =
                (x =>
                    x.Ground.Equals(m.Ground) && x.HasTeam(m.Home) && x.Date > m.Date.AddYears(relevantYearsDifference));
            Func<Match, bool> awayWherePredicate =
                (x =>
                    x.Ground.Equals(m.Ground) && x.HasTeam(m.Away) && x.Date > m.Date.AddYears(relevantYearsDifference));
            return ExtractInputSet(m, matches, term, homeWherePredicate, awayWherePredicate);
        }

        private static IEnumerable<double> ExtractStateScoreInputSet(Match m, List<Match> matches, int term)
        {
            const int relevantYearsDifference = -12;
            Func<Match, bool> homeWherePredicate =
                (x =>
                    x.Ground.State.Equals(m.Ground.State) && x.HasTeam(m.Home) &&
                    x.Date > m.Date.AddYears(relevantYearsDifference));
            Func<Match, bool> awayWherePredicate =
                (x =>
                    x.Ground.State.Equals(m.Ground.State) && x.HasTeam(m.Away) &&
                    x.Date > m.Date.AddYears(relevantYearsDifference));
            return ExtractInputSet(m, matches, term, homeWherePredicate, awayWherePredicate);
        }

        private static IEnumerable<double> ExtractDayScoreInputSet(Match m, List<Match> matches, int term)
        {
            Func<Match, bool> homeWherePredicate = (x => x.Date.DayOfWeek == m.Date.DayOfWeek && x.HasTeam(m.Home));
            Func<Match, bool> awayWherePredicate = (x => x.Date.DayOfWeek == m.Date.DayOfWeek && x.HasTeam(m.Away));
            return ExtractInputSet(m, matches, term, homeWherePredicate, awayWherePredicate);
        }

        private static IEnumerable<double> ExtractOpponentScoreSet(Match m, List<Match> matches,
            int term)
        {
            const int numOpponents = 6;
            var recentOpponentsHome =
                matches.Where(mtch => mtch.HasTeam(m.Home))
                    .OrderByDescending(x => x.Date)
                    .Take(numOpponents)
                    .Select(mtch => mtch.GetOpposition(m.Home))
                    .ToList();
            var recentOpponentsAway =
                matches.Where(mtch => mtch.HasTeam(m.Away))
                    .OrderByDescending(x => x.Date)
                    .Take(numOpponents)
                    .Select(mtch => mtch.GetOpposition(m.Away))
                    .ToList();

            Func<Match, bool> homeWherePredicate = (x => x.HasTeam(recentOpponentsHome) && !x.HasTeam(m.Home));
            Func<Match, bool> awayWherePredicate = (x => x.HasTeam(recentOpponentsAway) && !x.HasTeam(m.Away));
            return ExtractInputSet(m, matches, term, homeWherePredicate, awayWherePredicate);
        }

        private static IEnumerable<double> ExtractSharedOpponentScoreSet(Match m, List<Match> matches,
            int longTerm)
        {
            const int numOpponents = 15;
            var recentMatchesHome =
                matches.Where(mtch => mtch.HasTeam(m.Home) && !mtch.HasTeam(m.Away))
                    .OrderByDescending(mtch => mtch.Date)
                    .Take(numOpponents)
                    .ToList();
            var recentMatchesAway =
                matches.Where(mtch => mtch.HasTeam(m.Away) && !mtch.HasTeam(m.Home))
                    .OrderByDescending(mtch => mtch.Date)
                    .Take(numOpponents)
                    .ToList();

            Func<Match, bool> homeWherePredicate =
                (x => x.HasTeam(m.Home) && x.HasTeam(recentMatchesAway.Select(y => y.GetOpposition(m.Away)).ToList()));
            Func<Match, bool> awayWherePredicate =
                (x => x.HasTeam(m.Away) && x.HasTeam(recentMatchesHome.Select(y => y.GetOpposition(m.Home)).ToList()));
            return ExtractInputSet(m, matches, longTerm, homeWherePredicate, awayWherePredicate);
        }

        private static IEnumerable<double> ExtractInputSet(Match m, List<Match> matches, int term,
            Func<Match, bool> homeWherePredicate, Func<Match, bool> awayWherePredicate)
        {
            var inputSet = new List<double>
            {
                ExtractInput(matches, homeWherePredicate, term, (x => x.ScoreFor(m.Home).Goals), GetMaxSeasonGoals),
                ExtractInput(matches, homeWherePredicate, term, (x => x.ScoreFor(m.Home).Points), GetMaxSeasonPoints),
                ExtractInput(matches, awayWherePredicate, term, (x => x.ScoreFor(m.Away).Goals), GetMaxSeasonGoals),
                ExtractInput(matches, awayWherePredicate, term, (x => x.ScoreFor(m.Away).Points), GetMaxSeasonPoints),
                ExtractInput(matches, homeWherePredicate, term, (x => x.ScoreAgainst(m.Home).Goals), GetMaxSeasonGoals),
                ExtractInput(matches, homeWherePredicate, term, (x => x.ScoreAgainst(m.Home).Points), GetMaxSeasonPoints),
                ExtractInput(matches, awayWherePredicate, term, (x => x.ScoreAgainst(m.Away).Goals), GetMaxSeasonGoals),
                ExtractInput(matches, awayWherePredicate, term, (x => x.ScoreAgainst(m.Away).Points), GetMaxSeasonPoints)
            };

            return inputSet;
        }

        public static double ExtractInput(List<Match> s, Func<Match, bool> wherePredicate, int takeLength,
            Func<Match, double> sumSelector, Func<double, double> maxFunc)
        {
            var value = s
                .Where(wherePredicate)
                .OrderByDescending(x => x.Date)
                .Take(takeLength)
                .Sum(sumSelector);
            var max = maxFunc(
                s
                    .Where(wherePredicate)
                    .OrderByDescending(x => x.Date)
                    .Take(takeLength)
                    .Count());
            return Numbery.Normalise(value, max);
        }

        public static List<double> BuildInputs(Season s, Match m)
        {
            const int shortTerm = 3;
            const int longTerm = 10;
            const int longerTerm = 15;
            const int longestTerm = 20;

            var matches = s.GetMatches();


            var input = new List<double>();


            //Scores By Team
            foreach (var term in new List<int> {longTerm, shortTerm})
            {
                input.AddRange(ExtractTeamScoreInputSet(m, matches, term));
            }

            //Scores By Ground
            foreach (var term in new List<int> {longestTerm, shortTerm})
            {
                input.AddRange(ExtractGroundScoreInputSet(m, matches, term));
            }

            //Scores By State longerTerm
            foreach (var term in new List<int> {longestTerm, shortTerm})
            {
                input.AddRange(ExtractStateScoreInputSet(m, matches, term));
            }

            //Scores by Day
            foreach (var term in new List<int> {longerTerm, shortTerm})
            {
                input.AddRange(ExtractDayScoreInputSet(m, matches, term));
            }

            //Recent Opponents
            foreach (var term in new List<int> {longerTerm})
            {
                input.AddRange(ExtractOpponentScoreSet(m, matches, term));
            }

            //Recent Shared Opponents
            foreach (var term in new List<int> {longTerm})
            {
                input.AddRange(ExtractSharedOpponentScoreSet(m, matches, term));
            }

            return input;
        }

        public static double GetMaxSeasonGoals(double rounds)
        {
            return Util.MaxGoals*rounds;
        }

        public static double GetMaxSeasonPoints(double rounds)
        {
            return Util.MaxPoints*rounds;
        }

        public static String Printlayer(double[] vals)
        {
            var result = vals.Aggregate("{", (current, t) => current + String.Format("{0:N1}, ", t));
            return result + "}";
        }
    }
}
