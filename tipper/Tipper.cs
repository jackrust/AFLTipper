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
            Refresh(NumInputs, new List<int>() { DefaultHiddens }, NumOutputs);
        }

        public void Refresh(int maxEpochs, double targetError, int inputs, List<int> hiddens, int outputs)
        {
            Refresh(inputs, hiddens, outputs);
            Net.MaxEpochs = maxEpochs;
            Net.TargetError = targetError;
        }

        public void Refresh(int inputs, List<int> hiddens, int outputs)
        {
            Net = new FeedForwardNetwork(inputs, hiddens, outputs);
        }

        public Data LearnFromScratchFromTo(int fromYear, int fromRound, int toYear, int toRound)
        {
            Refresh(NumInputs, new List<int>() { DefaultHiddens }, NumOutputs);
            return LearnFromTo(fromYear, fromRound, toYear, toRound);
        }

        public Data LearnFromTo(RoundShell roundFrom, RoundShell roundTo)
        {
            return LearnFromTo(roundFrom.Year, roundFrom.Number, roundTo.Year, roundTo.Number);
        }

        public Data LearnFromTo(int fromYear, int fromRound, int toYear, int toRound)
        {
            var references = new List<string>();
            var inputs = new List<List<double>>();
            var targets = new List<List<double>>();
            var rounds = League.GetRounds(0, 0, toYear, toRound).Where(x => x.Matches.Count > 0).ToList();
            foreach (var m in rounds.Where(r => (r.Year == fromYear && r.Number >= fromRound) || (r.Year > fromYear)).SelectMany(r => r.Matches))
            {
                var season = new Season(toYear, rounds.Where(r => !r.Matches.Any(rm => rm.Date >= m.Date)).ToList());
                inputs.Add(BuildInputs(season, m));
                targets.Add(new List<double>()
                {
                    Numbery.Normalise(m.HomeScore().Goals, Util.MaxGoals),
                    Numbery.Normalise(m.HomeScore().Points, Util.MaxPoints),
                    Numbery.Normalise(m.AwayScore().Goals, Util.MaxGoals),
                    Numbery.Normalise(m.AwayScore().Points, Util.MaxPoints),
                });
                references.Add(m.Home.ApiName + " Vs " + m.Away.ApiName);
            }
            var data = new Data()
            {
                References = references,
                Inputs = inputs,
                Outputs = targets
            };
            return data;
        }

        public List<Match> Predict(RoundShell round)
        {
            return Predict(round.Year, round.Number, false);
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
                    new Ground(), new DateTime()));

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

        public static double ExtractInput(List<Match> s, Func<Match, bool> wherePredicate, int takeLength, Func<Match, double> sumSelector, Func<double, double> maxFunc)
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
            const int previous = 1;
            const int shortTerm = 3;
            const int midTerm = 5;
            const int longTerm = 7;
            const int longerTerm = 9;

            const int relevantYearsDifference = -6;

            var matches = s.GetMatches();


            var input = new List<double>();
            //Scores By Team longer. w -> 0.66
            input.Add(ExtractInput(matches, (x => x.HasTeam(m.Home)), longerTerm, (x => x.ScoreFor(m.Home).Goals), GetMaxSeasonGoals));
            input.Add(ExtractInput(matches, (x => x.HasTeam(m.Home)), longerTerm, (x => x.ScoreFor(m.Home).Points), GetMaxSeasonPoints));
            input.Add(ExtractInput(matches, (x => x.HasTeam(m.Away)), longerTerm, (x => x.ScoreFor(m.Away).Goals), GetMaxSeasonGoals));
            input.Add(ExtractInput(matches, (x => x.HasTeam(m.Away)), longerTerm, (x => x.ScoreFor(m.Away).Points), GetMaxSeasonPoints));

            input.Add(ExtractInput(matches, (x => x.HasTeam(m.Home)), longerTerm, (x => x.ScoreAgainst(m.Home).Goals), GetMaxSeasonGoals));
            input.Add(ExtractInput(matches, (x => x.HasTeam(m.Home)), longerTerm, (x => x.ScoreAgainst(m.Home).Points), GetMaxSeasonPoints));
            input.Add(ExtractInput(matches, (x => x.HasTeam(m.Away)), longerTerm, (x => x.ScoreAgainst(m.Away).Goals), GetMaxSeasonGoals));
            input.Add(ExtractInput(matches, (x => x.HasTeam(m.Away)), longerTerm, (x => x.ScoreAgainst(m.Away).Points), GetMaxSeasonPoints));

            //Scores By Team short. w -> 0.41
            input.Add(ExtractInput(matches, (x => x.HasTeam(m.Home)), shortTerm, (x => x.ScoreFor(m.Home).Goals), GetMaxSeasonGoals));
            input.Add(ExtractInput(matches, (x => x.HasTeam(m.Home)), shortTerm, (x => x.ScoreFor(m.Home).Points), GetMaxSeasonPoints));
            input.Add(ExtractInput(matches, (x => x.HasTeam(m.Away)), shortTerm, (x => x.ScoreFor(m.Away).Goals), GetMaxSeasonGoals));
            input.Add(ExtractInput(matches, (x => x.HasTeam(m.Away)), shortTerm, (x => x.ScoreFor(m.Away).Points), GetMaxSeasonPoints));

            input.Add(ExtractInput(matches, (x => x.HasTeam(m.Home)), shortTerm, (x => x.ScoreAgainst(m.Home).Goals), GetMaxSeasonGoals));
            input.Add(ExtractInput(matches, (x => x.HasTeam(m.Home)), shortTerm, (x => x.ScoreAgainst(m.Home).Points), GetMaxSeasonPoints));
            input.Add(ExtractInput(matches, (x => x.HasTeam(m.Away)), shortTerm, (x => x.ScoreAgainst(m.Away).Goals), GetMaxSeasonGoals));
            input.Add(ExtractInput(matches, (x => x.HasTeam(m.Away)), shortTerm, (x => x.ScoreAgainst(m.Away).Points), GetMaxSeasonPoints));

            //Scores By Ground
            input.Add(ExtractInput(matches, (x => x.Ground.Equals(m.Ground) && x.HasTeam(m.Home) && x.Date > m.Date.AddYears(relevantYearsDifference)), longTerm * 2, (x => x.ScoreFor(m.Home).Goals), GetMaxSeasonGoals));
            input.Add(ExtractInput(matches, (x => x.Ground.Equals(m.Ground) && x.HasTeam(m.Home) && x.Date > m.Date.AddYears(relevantYearsDifference)), longTerm * 2, (x => x.ScoreFor(m.Home).Points), GetMaxSeasonPoints));
            input.Add(ExtractInput(matches, (x => x.Ground.Equals(m.Ground) && x.HasTeam(m.Away) && x.Date > m.Date.AddYears(relevantYearsDifference)), longTerm * 2, (x => x.ScoreFor(m.Away).Goals), GetMaxSeasonGoals));
            input.Add(ExtractInput(matches, (x => x.Ground.Equals(m.Ground) && x.HasTeam(m.Away) && x.Date > m.Date.AddYears(relevantYearsDifference)), longTerm * 2, (x => x.ScoreFor(m.Away).Points), GetMaxSeasonPoints));

            input.Add(ExtractInput(matches, (x => x.Ground.Equals(m.Ground) && x.HasTeam(m.Home) && x.Date > m.Date.AddYears(relevantYearsDifference)), longTerm * 2, (x => x.ScoreAgainst(m.Home).Goals), GetMaxSeasonGoals));
            input.Add(ExtractInput(matches, (x => x.Ground.Equals(m.Ground) && x.HasTeam(m.Home) && x.Date > m.Date.AddYears(relevantYearsDifference)), longTerm * 2, (x => x.ScoreAgainst(m.Home).Points), GetMaxSeasonPoints));
            input.Add(ExtractInput(matches, (x => x.Ground.Equals(m.Ground) && x.HasTeam(m.Away) && x.Date > m.Date.AddYears(relevantYearsDifference)), longTerm * 2, (x => x.ScoreAgainst(m.Away).Goals), GetMaxSeasonGoals));
            input.Add(ExtractInput(matches, (x => x.Ground.Equals(m.Ground) && x.HasTeam(m.Away) && x.Date > m.Date.AddYears(relevantYearsDifference)), longTerm * 2, (x => x.ScoreAgainst(m.Away).Points), GetMaxSeasonPoints));

            //Scores by Day - 8
            input.Add(ExtractInput(matches, (x => x.Date.DayOfWeek == m.Date.DayOfWeek && x.HasTeam(m.Home)), longerTerm, (x => x.ScoreFor(m.Home).Goals), GetMaxSeasonGoals));
            input.Add(ExtractInput(matches, (x => x.Date.DayOfWeek == m.Date.DayOfWeek && x.HasTeam(m.Home)), longerTerm, (x => x.ScoreFor(m.Home).Points), GetMaxSeasonPoints));
            input.Add(ExtractInput(matches, (x => x.Date.DayOfWeek == m.Date.DayOfWeek && x.HasTeam(m.Away)), longerTerm, (x => x.ScoreFor(m.Away).Goals), GetMaxSeasonGoals));
            input.Add(ExtractInput(matches, (x => x.Date.DayOfWeek == m.Date.DayOfWeek && x.HasTeam(m.Away)), longerTerm, (x => x.ScoreFor(m.Away).Points), GetMaxSeasonPoints));

            input.Add(ExtractInput(matches, (x => x.Date.DayOfWeek == m.Date.DayOfWeek && x.HasTeam(m.Home)), longerTerm, (x => x.ScoreAgainst(m.Home).Goals), GetMaxSeasonGoals));
            input.Add(ExtractInput(matches, (x => x.Date.DayOfWeek == m.Date.DayOfWeek && x.HasTeam(m.Home)), longerTerm, (x => x.ScoreAgainst(m.Home).Points), GetMaxSeasonPoints));
            input.Add(ExtractInput(matches, (x => x.Date.DayOfWeek == m.Date.DayOfWeek && x.HasTeam(m.Away)), longerTerm, (x => x.ScoreAgainst(m.Away).Goals), GetMaxSeasonGoals));
            input.Add(ExtractInput(matches, (x => x.Date.DayOfWeek == m.Date.DayOfWeek && x.HasTeam(m.Away)), longerTerm, (x => x.ScoreAgainst(m.Away).Points), GetMaxSeasonPoints));

            //Recent Opponents
            var recentOpponentsHome = matches.Where(mtch => mtch.HasTeam(m.Home)).OrderByDescending(x => x.Date).Take(midTerm).Select(mtch => mtch.GetOpposition(m.Home)).ToList();
            var recentOpponentsAway = matches.Where(mtch => mtch.HasTeam(m.Away)).OrderByDescending(x => x.Date).Take(midTerm).Select(mtch => mtch.GetOpposition(m.Away)).ToList();

            //Recent Opponents
            input.Add(ExtractInput(matches, (mtch => mtch.HasTeam(recentOpponentsHome) && !mtch.HasTeam(m.Home)), longerTerm, (x => x.ScoreFor(recentOpponentsHome).Goals), GetMaxSeasonGoals));
            input.Add(ExtractInput(matches, (mtch => mtch.HasTeam(recentOpponentsHome) && !mtch.HasTeam(m.Home)), longerTerm, (x => x.ScoreFor(recentOpponentsHome).Points), GetMaxSeasonPoints));
            input.Add(ExtractInput(matches, (mtch => mtch.HasTeam(recentOpponentsAway) && !mtch.HasTeam(m.Away)), longerTerm, (x => x.ScoreFor(recentOpponentsAway).Goals), GetMaxSeasonGoals));
            input.Add(ExtractInput(matches, (mtch => mtch.HasTeam(recentOpponentsAway) && !mtch.HasTeam(m.Away)), longerTerm, (x => x.ScoreFor(recentOpponentsAway).Points), GetMaxSeasonPoints));

            input.Add(ExtractInput(matches, (mtch => mtch.HasTeam(recentOpponentsHome) && !mtch.HasTeam(m.Home)), longerTerm, (x => x.ScoreAgainst(recentOpponentsHome).Goals), GetMaxSeasonGoals));
            input.Add(ExtractInput(matches, (mtch => mtch.HasTeam(recentOpponentsHome) && !mtch.HasTeam(m.Home)), longerTerm, (x => x.ScoreAgainst(recentOpponentsHome).Points), GetMaxSeasonPoints));
            input.Add(ExtractInput(matches, (mtch => mtch.HasTeam(recentOpponentsAway) && !mtch.HasTeam(m.Away)), longerTerm, (x => x.ScoreAgainst(recentOpponentsAway).Goals), GetMaxSeasonGoals));
            input.Add(ExtractInput(matches, (mtch => mtch.HasTeam(recentOpponentsAway) && !mtch.HasTeam(m.Away)), longerTerm, (x => x.ScoreAgainst(recentOpponentsAway).Points), GetMaxSeasonPoints));


            //Recent Shared Opponents
            var recentMatchesHome = matches.Where(mtch => mtch.HasTeam(m.Home) && !mtch.HasTeam(m.Away)).OrderByDescending(mtch => mtch.Date).Take(longerTerm).ToList();
            var recentMatchesAway = matches.Where(mtch => mtch.HasTeam(m.Away) && !mtch.HasTeam(m.Home)).OrderByDescending(mtch => mtch.Date).Take(longerTerm).ToList();
            var recentMatchesSharedOpponentHome = recentMatchesHome.Where(hmtch => hmtch.HasTeam(recentMatchesAway.Select(amtch => amtch.GetOpposition(m.Away)).ToList())).ToList();
            var recentMatchesSharedOpponentAway = recentMatchesAway.Where(amtch => amtch.HasTeam(recentMatchesHome.Select(hmtch => hmtch.GetOpposition(m.Home)).ToList())).ToList();

            input.Add(ExtractInput(recentMatchesSharedOpponentHome, (x => x.HasTeam(m.Home)), shortTerm, (x => x.ScoreFor(m.Home).Goals), GetMaxSeasonGoals));
            input.Add(ExtractInput(recentMatchesSharedOpponentHome, (x => x.HasTeam(m.Home)), shortTerm, (x => x.ScoreFor(m.Home).Points), GetMaxSeasonPoints));
            input.Add(ExtractInput(recentMatchesSharedOpponentAway, (x => x.HasTeam(m.Away)), shortTerm, (x => x.ScoreFor(m.Away).Goals), GetMaxSeasonGoals));
            input.Add(ExtractInput(recentMatchesSharedOpponentAway, (x => x.HasTeam(m.Away)), shortTerm, (x => x.ScoreFor(m.Away).Points), GetMaxSeasonPoints));

            input.Add(ExtractInput(recentMatchesSharedOpponentHome, (x => x.HasTeam(m.Home)), shortTerm, (x => x.ScoreAgainst(m.Home).Goals), GetMaxSeasonGoals));
            input.Add(ExtractInput(recentMatchesSharedOpponentHome, (x => x.HasTeam(m.Home)), shortTerm, (x => x.ScoreAgainst(m.Home).Points), GetMaxSeasonPoints));
            input.Add(ExtractInput(recentMatchesSharedOpponentAway, (x => x.HasTeam(m.Away)), shortTerm, (x => x.ScoreAgainst(m.Away).Goals), GetMaxSeasonGoals));
            input.Add(ExtractInput(recentMatchesSharedOpponentAway, (x => x.HasTeam(m.Away)), shortTerm, (x => x.ScoreAgainst(m.Away).Points), GetMaxSeasonPoints));

            //Recent Shared Opponents
            input.Add(ExtractInput(recentMatchesSharedOpponentHome, (x => x.HasTeam(m.Home)), midTerm, (x => x.ScoreFor(m.Home).Goals), GetMaxSeasonGoals));
            input.Add(ExtractInput(recentMatchesSharedOpponentHome, (x => x.HasTeam(m.Home)), midTerm, (x => x.ScoreFor(m.Home).Points), GetMaxSeasonPoints));
            input.Add(ExtractInput(recentMatchesSharedOpponentAway, (x => x.HasTeam(m.Away)), midTerm, (x => x.ScoreFor(m.Away).Goals), GetMaxSeasonGoals));
            input.Add(ExtractInput(recentMatchesSharedOpponentAway, (x => x.HasTeam(m.Away)), midTerm, (x => x.ScoreFor(m.Away).Points), GetMaxSeasonPoints));

            input.Add(ExtractInput(recentMatchesSharedOpponentHome, (x => x.HasTeam(m.Home)), midTerm, (x => x.ScoreAgainst(m.Home).Goals), GetMaxSeasonGoals));
            input.Add(ExtractInput(recentMatchesSharedOpponentHome, (x => x.HasTeam(m.Home)), midTerm, (x => x.ScoreAgainst(m.Home).Points), GetMaxSeasonPoints));
            input.Add(ExtractInput(recentMatchesSharedOpponentAway, (x => x.HasTeam(m.Away)), midTerm, (x => x.ScoreAgainst(m.Away).Goals), GetMaxSeasonGoals));
            input.Add(ExtractInput(recentMatchesSharedOpponentAway, (x => x.HasTeam(m.Away)), midTerm, (x => x.ScoreAgainst(m.Away).Points), GetMaxSeasonPoints));

            //Recent Shared Opponents
            input.Add(ExtractInput(recentMatchesSharedOpponentHome, (x => x.HasTeam(m.Home)), longerTerm, (x => x.ScoreFor(m.Home).Goals), GetMaxSeasonGoals));
            input.Add(ExtractInput(recentMatchesSharedOpponentHome, (x => x.HasTeam(m.Home)), longerTerm, (x => x.ScoreFor(m.Home).Points), GetMaxSeasonPoints));
            input.Add(ExtractInput(recentMatchesSharedOpponentAway, (x => x.HasTeam(m.Away)), longerTerm, (x => x.ScoreFor(m.Away).Goals), GetMaxSeasonGoals));
            input.Add(ExtractInput(recentMatchesSharedOpponentAway, (x => x.HasTeam(m.Away)), longerTerm, (x => x.ScoreFor(m.Away).Points), GetMaxSeasonPoints));

            input.Add(ExtractInput(recentMatchesSharedOpponentHome, (x => x.HasTeam(m.Home)), longerTerm, (x => x.ScoreAgainst(m.Home).Goals), GetMaxSeasonGoals));
            input.Add(ExtractInput(recentMatchesSharedOpponentHome, (x => x.HasTeam(m.Home)), longerTerm, (x => x.ScoreAgainst(m.Home).Points), GetMaxSeasonPoints));
            input.Add(ExtractInput(recentMatchesSharedOpponentAway, (x => x.HasTeam(m.Away)), longerTerm, (x => x.ScoreAgainst(m.Away).Goals), GetMaxSeasonGoals));
            input.Add(ExtractInput(recentMatchesSharedOpponentAway, (x => x.HasTeam(m.Away)), longerTerm, (x => x.ScoreAgainst(m.Away).Points), GetMaxSeasonPoints));

            //Streak by Team - 2
            input.Add(ExtractInput(matches, (x => x.HasTeam(m.Home)), longTerm, (x => x.ScoreFor(m.Home).Total() > x.ScoreAgainst(m.Home).Total() ? 1 : 0), SimpleCount));
            input.Add(ExtractInput(matches, (x => x.HasTeam(m.Away)), longTerm, (x => x.ScoreFor(m.Away).Total() > x.ScoreAgainst(m.Away).Total() ? 1 : 0), SimpleCount));

            //Experience at Ground
            input.Add(ExtractInput(matches, (x => x.HasTeam(m.Home) && x.Date > m.Date.AddYears(relevantYearsDifference)), longerTerm * 2, (x => x.Ground.Equals(m.Ground) ? 1 : 0), SimpleCount));
            input.Add(ExtractInput(matches, (x => x.HasTeam(m.Away) && x.Date > m.Date.AddYears(relevantYearsDifference)), longerTerm * 2, (x => x.Ground.Equals(m.Ground) ? 1 : 0), SimpleCount));

            //Days between games
            var previousGameHome = matches.Where(x => x.HasTeam(m.Home)).OrderByDescending(x => x.Date).FirstOrDefault();
            var previousGameAway = matches.Where(x => x.HasTeam(m.Away)).OrderByDescending(x => x.Date).FirstOrDefault();
            var datediffHome = previousGameHome == null ? GetMaxDaysBetweenGames() : (m.Date - previousGameHome.Date).Days;
            var datediffAway = previousGameAway == null ? GetMaxDaysBetweenGames() : (m.Date - previousGameAway.Date).Days;
            input.Add(Numbery.Normalise(datediffHome, GetMaxDaysBetweenGames()));
            input.Add(Numbery.Normalise(datediffAway, GetMaxDaysBetweenGames()));

            return input;
        }

        public int MarkRound(int round, List<Match> tips)
        {
            var matches = League.GetCurrentSeason().Rounds[round].Matches;
            return tips.Count != matches.Count ? 0 : matches.Sum(t => tips.Count(t1 => t.GetLosingTeam().Equals(t1.GetLosingTeam())));
        }

        public int MarkRound(int year, int round, List<Match> tips)
        {
            var matches = League.GetSeason(year).Rounds[round].Matches;
            return tips.Count != matches.Count ? 0 : matches.Sum(t => tips.Count(t1 => t.GetLosingTeam().Equals(t1.GetLosingTeam())));
        }

        public static double GetMaxLadderPoints(double rounds)
        {
            return rounds * 4;
        }
        public static double GetMaxRLadderPoints(double rounds)
        {
            return rounds * 4 + rounds * rounds * 2;
        }
        public static double GetMaxCLadderPoints(double rounds)
        {
            return Numbery.Factorial(rounds);
        }
        public static double GetMaxSeasonScores(double rounds)
        {
            return Util.MaxScore * rounds;
        }
        public static double GetMaxSeasonGoals(double rounds)
        {
            return Util.MaxGoals * rounds;
        }
        public static double GetMaxSeasonPoints(double rounds)
        {
            return Util.MaxPoints * rounds;
        }

        public static double SimpleCount(double rounds)
        {
            return rounds;
        }

        private static int GetMaxDaysBetweenGames()
        {
            return 21;//TODO: find the actual max in season
        }

        public static String Printlayer(double[] vals)
        {
            var result = vals.Aggregate("{", (current, t) => current + String.Format("{0:N1}, ", t));
            return result + "}";
        }
    }
}
