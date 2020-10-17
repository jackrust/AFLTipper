using System;
using System.Collections.Generic;
using System.Linq;
using ArtificialNeuralNetwork.DataManagement;
using AustralianRulesFootball;
using Utilities;

namespace Tipper
{
    public abstract class AFLDataInterpreter
    {
        public int DataExpiryDays = 720;

        #region inperpretations
        public struct InterpretationSubsets
        {
            public static List<int> DefaultInterpretationSubset = new List<int> { 1, 5, 11, 19, 29 };
        }

        public struct Interpretations
        {
            //If Interpretation change network will need to change too
            public static List<List<int>> BespokeApiInterpretation = new List<List<int>>
            {
                new List<int> { 9, 13, 17 },
                new List<int> { 25, 31, 37 },
                new List<int> { 1, 3, 5 },
                new List<int> { 25, 31, 37 },
                new List<int> { 25, 31, 37 }
            };

            public static List<List<int>> BespokeLegacyInterpretation = new List<List<int>>
            {
                new List<int> {1, 8, 21},
                new List<int> {1, 8, 21},
                new List<int> {1, 8, 21},
                new List<int> {1, 8, 21},
                new List<int> {1, 8, 21}
            };


            public static List<List<int>> DefaultInterpretation = new List<List<int>>
            {
                InterpretationSubsets.DefaultInterpretationSubset,
                InterpretationSubsets.DefaultInterpretationSubset,
                InterpretationSubsets.DefaultInterpretationSubset,
                InterpretationSubsets.DefaultInterpretationSubset,
                InterpretationSubsets.DefaultInterpretationSubset,
                InterpretationSubsets.DefaultInterpretationSubset
            };
        }
        #endregion

        #region DataPoint
        public DataPoint BuildDataPoint(List<Match> history, Match m)
        {
            return BuildDataPoint(history, m, Interpretations.DefaultInterpretation);
        }

        public DataPoint BuildDataPoint(List<Match> history, Match m, List<List<int>> inputInpertretation)
        {
            var datapoint = new DataPoint
            {
                Inputs = (BuildInputs(history, m, inputInpertretation)),
                Outputs = BuildOutputs(m).ToList(),
                Reference = m.ToTuple()
            };
            return datapoint;
        }
        #endregion

        #region Inputs
        public List<double> BuildInputs(List<Match> history, Match m, List<List<int>> interpretation)
        {
            var input = new List<double>();
            
            //V1 - measure by Score

            //Scores By Team
            if (interpretation.Count < 1)
                return input;
            foreach (var term in interpretation[0])
            {
                if(term > 0)
                    input.AddRange(ExtractTeamScoreInputSet(m, history, term, ExtractInputSetForScore));
            }//12

            //Scores By Ground
            if (interpretation.Count < 2)
                return input;
            foreach (var term in interpretation[1])
            {
                if (term > 0)
                    input.AddRange(ExtractGroundScoreInputSet(m, history, term, ExtractInputSetForScore));
            }//24

            //Scores By State longerTerm
            if (interpretation.Count < 3)
                return input;
            foreach (var term in interpretation[2])
            {
                if (term > 0)
                    input.AddRange(ExtractStateScoreInputSet(m, history, term, ExtractInputSetForScore));
            }//36

            //Scores by Day
            if (interpretation.Count < 4)
                return input;
            foreach (var term in interpretation[3])
            {
                if (term > 0)
                    input.AddRange(ExtractDayScoreInputSet(m, history, term, ExtractInputSetForScore));
            }//48

            //Recent Shared Opponents
            if (interpretation.Count < 5)
                return input;
            foreach (var term in interpretation[4])
            {
                if (term > 0)
                    input.AddRange(ExtractSharedOpponentScoreSet(m, history, term, ExtractInputSetForScore));
            }//60

            //Scores by quality of recent Opponents
            if (interpretation.Count < 6)
                return input;
            foreach (var term in interpretation[5])
            {
                if (term > 0)
                    input.AddRange(ExtractQualityOfRecentOpponentScoreSet(m, history, term, ExtractInputSetForOppositionScore));
            }//72

            //Outcome focus
            //Wins By Team
            if (interpretation.Count < 7)
                return input;
            foreach (var term in interpretation[6])
            {
                if (term > 0)
                    input.AddRange(ExtractTeamWinInputSet(m, history, term, ExtractInputSetForWin));
            }//60

            //Match stats
            //Kicks
            if (interpretation.Count < 8)
                return input;
            foreach (var term in interpretation[7])
            {
                if (term > 0)
                    input.AddRange(ExtractKicksInputSet(m, history, term));
            }//164

            //Handballs
            if (interpretation.Count < 9)
                return input;
            foreach (var term in interpretation[8])
            {
                if (term > 0)
                    input.AddRange(ExtractHandballsInputSet(m, history, term));
            }//240

            //Marks
            if (interpretation.Count < 10)
                return input;
            foreach (var term in interpretation[9])
            {
                if (term > 0)
                    input.AddRange(ExtractMarksInputSet(m, history, term));
            }//318

            //Hitouts
            if (interpretation.Count < 11)
                return input;
            foreach (var term in interpretation[10])
            {
                if (term > 0)
                    input.AddRange(ExtractHitoutsInputSet(m, history, term));
            }//396

            //Tackles
            if (interpretation.Count < 12)
                return input;
            foreach (var term in interpretation[11])
            {
                if (term > 0)
                    input.AddRange(ExtractTacklesInputSet(m, history, term));
            }//474

            //Frees
            if (interpretation.Count < 13)
                return input;
            foreach (var term in interpretation[12])
            {
                if (term > 0)
                    input.AddRange(ExtractFreesInputSet(m, history, term));
            }//630

            return input;
        }

        private IEnumerable<double> ExtractTeamWinInputSet(Match m, List<Match> matches, int term, Func<Match, List<Match>, int, Func<Match, bool>, Func<Match, bool>, IEnumerable<double>> extrator)
        {
            Func<Match, bool> homeWherePredicate = (x => x.HasTeam(m.Home) && x.Date > m.Date.AddDays(-DataExpiryDays));
            Func<Match, bool> awayWherePredicate = (x => x.HasTeam(m.Away) && x.Date > m.Date.AddDays(-DataExpiryDays));
            return extrator(m, matches, term, homeWherePredicate, awayWherePredicate);
        }

        private IEnumerable<double> ExtractTeamScoreInputSet(Match m, List<Match> matches, int term, Func<Match, List<Match>, int, Func<Match, bool>, Func<Match, bool>, IEnumerable<double>> extrator)
        {
            Func<Match, bool> homeWherePredicate = (x => x.HasTeam(m.Home) && x.Date > m.Date.AddDays(-DataExpiryDays));
            Func<Match, bool> awayWherePredicate = (x => x.HasTeam(m.Away) && x.Date > m.Date.AddDays(-DataExpiryDays));
            return extrator(m, matches, term, homeWherePredicate, awayWherePredicate);
        }

        private IEnumerable<double> ExtractGroundScoreInputSet(Match m, List<Match> matches, int term, Func<Match, List<Match>, int, Func<Match, bool>, Func<Match, bool>, IEnumerable<double>> extrator)
        {
            const int relevantYearsDifference = -12;

            Func<Match, bool> homeWherePredicate =
                (x =>
                    x.Ground.Equals(m.Ground) && x.HasTeam(m.Home) && x.Date > m.Date.AddYears(relevantYearsDifference) && x.Date > m.Date.AddDays(-DataExpiryDays));
            Func<Match, bool> awayWherePredicate =
                (x =>
                    x.Ground.Equals(m.Ground) && x.HasTeam(m.Away) && x.Date > m.Date.AddYears(relevantYearsDifference) && x.Date > m.Date.AddDays(-DataExpiryDays));
            return extrator(m, matches, term, homeWherePredicate, awayWherePredicate);
        }

        private IEnumerable<double> ExtractStateScoreInputSet(Match m, List<Match> matches, int term, Func<Match, List<Match>, int, Func<Match, bool>, Func<Match, bool>, IEnumerable<double>> extrator)
        {
            const int relevantYearsDifference = -6;
            Func<Match, bool> homeWherePredicate =
                (x =>
                    x.Ground.State.Equals(m.Ground.State) && x.HasTeam(m.Home) &&
                    x.Date > m.Date.AddYears(relevantYearsDifference) && x.Date > m.Date.AddDays(-DataExpiryDays));
            Func<Match, bool> awayWherePredicate =
                (x =>
                    x.Ground.State.Equals(m.Ground.State) && x.HasTeam(m.Away) &&
                    x.Date > m.Date.AddYears(relevantYearsDifference) && x.Date > m.Date.AddDays(-DataExpiryDays));
            return extrator(m, matches, term, homeWherePredicate, awayWherePredicate);
        }

        private IEnumerable<double> ExtractDayScoreInputSet(Match m, List<Match> matches, int term, Func<Match, List<Match>, int, Func<Match, bool>, Func<Match, bool>, IEnumerable<double>> extrator)
        {
            Func<Match, bool> homeWherePredicate = (x => x.Date.DayOfWeek == m.Date.DayOfWeek && x.HasTeam(m.Home) && x.Date > m.Date.AddDays(-DataExpiryDays));
            Func<Match, bool> awayWherePredicate = (x => x.Date.DayOfWeek == m.Date.DayOfWeek && x.HasTeam(m.Away) && x.Date > m.Date.AddDays(-DataExpiryDays));
            return extrator(m, matches, term, homeWherePredicate, awayWherePredicate);
        }

        private IEnumerable<double> ExtractSharedOpponentScoreSet(Match m, List<Match> matches,
            int term, Func<Match, List<Match>, int, Func<Match, bool>, Func<Match, bool>, IEnumerable<double>> extrator)
        {
            const int numOpponents = 24;
            var recentMatchesHome =
                matches.Where(match => match.HasTeam(m.Home) && !match.HasTeam(m.Away))
                    .OrderByDescending(match => match.Date)
                    .Take(numOpponents)
                    .ToList();
            var recentMatchesAway =
                matches.Where(match => match.HasTeam(m.Away) && !match.HasTeam(m.Home))
                    .OrderByDescending(match => match.Date)
                    .Take(numOpponents)
                    .ToList();

            Func<Match, bool> homeWherePredicate =
                (x => x.HasTeam(m.Home) &&
                      x.HasTeam(recentMatchesAway.Select(y => y.GetOpposition(m.Away)).ToList()) &&
                      x.Date > m.Date.AddDays(-DataExpiryDays));
            Func<Match, bool> awayWherePredicate =
                (x => x.HasTeam(m.Away) && 
                      x.HasTeam(recentMatchesHome.Select(y => y.GetOpposition(m.Home)).ToList()) &&
                      x.Date > m.Date.AddDays(-DataExpiryDays));
            return extrator(m, matches, term, homeWherePredicate, awayWherePredicate);
        }

        private IEnumerable<double> ExtractQualityOfRecentOpponentScoreSet(Match m, List<Match> matches,
            int term,  Func<int,
            List<Tuple<Score, Score, DateTime>>,
            List<Tuple<Score, Score, DateTime>>, IEnumerable<double>> extractor)
        {
            const int numOpponents = 24;
            var recentHomeMatches =
                matches.Where(match => match.HasTeam(m.Home) && !match.HasTeam(m.Away))
                    .OrderByDescending(match => match.Date)
                    .Take(numOpponents)
                    .ToList();

            var recentAwayMatches =
                matches.Where(match => match.HasTeam(m.Away) && !match.HasTeam(m.Home))
                    .OrderByDescending(match => match.Date)
                    .Take(numOpponents)
                    .ToList();

            //Foreach recent game get some recent matches by that team
            var recentHomeRecentOpponentMatches = new List<Tuple<Score, Score, DateTime>>();
            foreach (var recentHomeMatch in recentHomeMatches)
            {
                var opposition = recentHomeMatch.GetOpposition(m.Home);
                foreach (var match in matches.Where(match => match.HasTeam(opposition) && !match.HasTeam(m.Home) && match.Date <= recentHomeMatch.Date.AddMonths(1) && match.Date >= recentHomeMatch.Date.AddMonths(-1)))
                {
                    var oppositionInQuestionScore = match.GetTeamScore(opposition);
                    var oppositionOfOppositionScore = match.GetOppositionScore(opposition);
                    var date = match.Date;
                    var tuple = new Tuple<Score, Score, DateTime>(oppositionInQuestionScore, oppositionOfOppositionScore, date);
                    recentHomeRecentOpponentMatches.Add(tuple);
                }
            }

            var recentAwayRecentOpponentMatches = new List<Tuple<Score, Score, DateTime>>();
            foreach (var recentAwayMatch in recentAwayMatches)
            {
                var opposition = recentAwayMatch.GetOpposition(m.Away);
                foreach (var match in matches.Where(match => match.HasTeam(opposition) && !match.HasTeam(m.Away) && match.Date <= recentAwayMatch.Date.AddMonths(1) && match.Date >= recentAwayMatch.Date.AddMonths(-1)))
                {
                    var oppositionInQuestionScore = match.GetTeamScore(opposition);
                    var oppositionOfOppositionScore = match.GetOppositionScore(opposition);
                    var date = match.Date;
                    var tuple = new Tuple<Score, Score, DateTime>(oppositionInQuestionScore, oppositionOfOppositionScore, date);
                    recentAwayRecentOpponentMatches.Add(tuple);
                }
            }

            return extractor(term, recentHomeRecentOpponentMatches, recentAwayRecentOpponentMatches);
        }

        /*private IEnumerable<double> ExtractKicksInputSet(Match m, List<Match> matches, int term)
        {
            var inputSet = new List<double>();
            var ht = matches.Where(x => x.HasTeam(m.Home) && x.Date < m.Date).OrderByDescending(x => x.Date).Take(term).Select(x => Numbery.Normalise(x.GetTeamStats(m.Home).Kicks, MatchStatistics.MIN_KICKS, MatchStatistics.MAX_KICKS, 0.0, 1.0)).ToList();
            for (var i = 0; i < ht.Count(); i++)
                ht.Insert(0, 0.0);
            var at = matches.Where(x => x.HasTeam(m.Away) && x.Date < m.Date).OrderByDescending(x => x.Date).Take(term).Select(x => Numbery.Normalise(x.GetTeamStats(m.Away).Kicks, MatchStatistics.MIN_KICKS, MatchStatistics.MAX_KICKS, 0.0, 1.0)).ToList();
            for (var i = 0; i < at.Count(); i++)
                at.Insert(0, 0.0);
            var ho = matches.Where(x => x.HasTeam(m.Home) && x.Date < m.Date).OrderByDescending(x => x.Date).Take(term).Select(x => Numbery.Normalise(x.GetOppositionStats(m.Home).Kicks, MatchStatistics.MIN_KICKS, MatchStatistics.MAX_KICKS, 0.0, 1.0)).ToList();
            for (var i = 0; i < ho.Count(); i++)
                ho.Insert(0, 0.0);
            var ao = matches.Where(x => x.HasTeam(m.Away) && x.Date < m.Date).OrderByDescending(x => x.Date).Take(term).Select(x => Numbery.Normalise(x.GetOppositionStats(m.Away).Kicks, MatchStatistics.MIN_KICKS, MatchStatistics.MAX_KICKS, 0.0, 1.0)).ToList();
            for (var i = 0; i < ao.Count(); i++)
                ao.Insert(0, 0.0);
            inputSet.AddRange(ht);
            inputSet.AddRange(at);
            inputSet.AddRange(ho);
            inputSet.AddRange(ao);
            return inputSet;
        }

        private IEnumerable<double> ExtractHandballsInputSet(Match m, List<Match> matches, int term)
        {
            var inputSet = new List<double>();
            var ht = matches.Where(x => x.HasTeam(m.Home) && x.Date < m.Date).OrderByDescending(x => x.Date).Take(term).Select(x => Numbery.Normalise(x.GetTeamStats(m.Home).Handballs, MatchStatistics.MIN_HANDBALLS, MatchStatistics.MAX_HANDBALLS, 0.0, 1.0)).ToList();
            for (var i = 0; i < ht.Count(); i++)
                ht.Insert(0, 0.0);
            var at = matches.Where(x => x.HasTeam(m.Away) && x.Date < m.Date).OrderByDescending(x => x.Date).Take(term).Select(x => Numbery.Normalise(x.GetTeamStats(m.Away).Handballs, MatchStatistics.MIN_HANDBALLS, MatchStatistics.MAX_HANDBALLS, 0.0, 1.0)).ToList();
            for (var i = 0; i < at.Count(); i++)
                at.Insert(0, 0.0);
            var ho = matches.Where(x => x.HasTeam(m.Home) && x.Date < m.Date).OrderByDescending(x => x.Date).Take(term).Select(x => Numbery.Normalise(x.GetOppositionStats(m.Home).Handballs, MatchStatistics.MIN_HANDBALLS, MatchStatistics.MAX_HANDBALLS, 0.0, 1.0)).ToList();
            for (var i = 0; i < ho.Count(); i++)
                ho.Insert(0, 0.0);
            var ao = matches.Where(x => x.HasTeam(m.Away) && x.Date < m.Date).OrderByDescending(x => x.Date).Take(term).Select(x => Numbery.Normalise(x.GetOppositionStats(m.Away).Handballs, MatchStatistics.MIN_HANDBALLS, MatchStatistics.MAX_HANDBALLS, 0.0, 1.0)).ToList();
            for (var i = 0; i < ao.Count(); i++)
                ao.Insert(0, 0.0);
            inputSet.AddRange(ht);
            inputSet.AddRange(at);
            inputSet.AddRange(ho);
            inputSet.AddRange(ao);
            return inputSet;
        }

        private IEnumerable<double> ExtractMarksInputSet(Match m, List<Match> matches, int term)
        {
            var inputSet = new List<double>();
            var ht = matches.Where(x => x.HasTeam(m.Home) && x.Date < m.Date).OrderByDescending(x => x.Date).Take(term).Select(x => Numbery.Normalise(x.GetTeamStats(m.Home).Marks, MatchStatistics.MIN_MARKS, MatchStatistics.MAX_MARKS, 0.0, 1.0)).ToList();
            for (var i = 0; i < ht.Count(); i++)
                ht.Insert(0, 0.0);
            var at = matches.Where(x => x.HasTeam(m.Away) && x.Date < m.Date).OrderByDescending(x => x.Date).Take(term).Select(x => Numbery.Normalise(x.GetTeamStats(m.Away).Marks, MatchStatistics.MIN_MARKS, MatchStatistics.MAX_MARKS, 0.0, 1.0)).ToList();
            for (var i = 0; i < at.Count(); i++)
                at.Insert(0, 0.0);
            var ho = matches.Where(x => x.HasTeam(m.Home) && x.Date < m.Date).OrderByDescending(x => x.Date).Take(term).Select(x => Numbery.Normalise(x.GetOppositionStats(m.Home).Marks, MatchStatistics.MIN_MARKS, MatchStatistics.MAX_MARKS, 0.0, 1.0)).ToList();
            for (var i = 0; i < ho.Count(); i++)
                ho.Insert(0, 0.0);
            var ao = matches.Where(x => x.HasTeam(m.Away) && x.Date < m.Date).OrderByDescending(x => x.Date).Take(term).Select(x => Numbery.Normalise(x.GetOppositionStats(m.Away).Marks, MatchStatistics.MIN_MARKS, MatchStatistics.MAX_MARKS, 0.0, 1.0)).ToList();
            for (var i = 0; i < ao.Count(); i++)
                ao.Insert(0, 0.0);
            inputSet.AddRange(ht);
            inputSet.AddRange(at);
            inputSet.AddRange(ho);
            inputSet.AddRange(ao);
            return inputSet;
        }

        private IEnumerable<double> ExtractHitoutsInputSet(Match m, List<Match> matches, int term)
        {
            var inputSet = new List<double>();
            var ht = matches.Where(x => x.HasTeam(m.Home) && x.Date < m.Date).OrderByDescending(x => x.Date).Take(term).Select(x => Numbery.Normalise(x.GetTeamStats(m.Home).HitOuts, MatchStatistics.MIN_HITOUTS, MatchStatistics.MAX_HITOUTS, 0.0, 1.0)).ToList();
            for (var i = 0; i < ht.Count(); i++)
                ht.Insert(0, 0.0);
            var at = matches.Where(x => x.HasTeam(m.Away) && x.Date < m.Date).OrderByDescending(x => x.Date).Take(term).Select(x => Numbery.Normalise(x.GetTeamStats(m.Away).HitOuts, MatchStatistics.MIN_HITOUTS, MatchStatistics.MAX_HITOUTS, 0.0, 1.0)).ToList();
            for (var i = 0; i < at.Count(); i++)
                at.Insert(0, 0.0);
            var ho = matches.Where(x => x.HasTeam(m.Home) && x.Date < m.Date).OrderByDescending(x => x.Date).Take(term).Select(x => Numbery.Normalise(x.GetOppositionStats(m.Home).HitOuts, MatchStatistics.MIN_HITOUTS, MatchStatistics.MAX_HITOUTS, 0.0, 1.0)).ToList();
            for (var i = 0; i < ho.Count(); i++)
                ho.Insert(0, 0.0);
            var ao = matches.Where(x => x.HasTeam(m.Away) && x.Date < m.Date).OrderByDescending(x => x.Date).Take(term).Select(x => Numbery.Normalise(x.GetOppositionStats(m.Away).HitOuts, MatchStatistics.MIN_HITOUTS, MatchStatistics.MAX_HITOUTS, 0.0, 1.0)).ToList();
            for (var i = 0; i < ao.Count(); i++)
                ao.Insert(0, 0.0);
            inputSet.AddRange(ht);
            inputSet.AddRange(at);
            inputSet.AddRange(ho);
            inputSet.AddRange(ao);
            return inputSet;
        }

        private IEnumerable<double> ExtractTacklesInputSet(Match m, List<Match> matches, int term)
        {
            var inputSet = new List<double>();
            var ht = matches.Where(x => x.HasTeam(m.Home) && x.Date < m.Date).OrderByDescending(x => x.Date).Take(term).Select(x => Numbery.Normalise(x.GetTeamStats(m.Home).Tackles, MatchStatistics.MIN_TACKLES, MatchStatistics.MAX_TACKLES, 0.0, 1.0)).ToList();
            for (var i = 0; i < ht.Count(); i++)
                ht.Insert(0, 0.0);
            var at = matches.Where(x => x.HasTeam(m.Away) && x.Date < m.Date).OrderByDescending(x => x.Date).Take(term).Select(x => Numbery.Normalise(x.GetTeamStats(m.Away).Tackles, MatchStatistics.MIN_TACKLES, MatchStatistics.MAX_TACKLES, 0.0, 1.0)).ToList();
            for (var i = 0; i < at.Count(); i++)
                at.Insert(0, 0.0);
            var ho = matches.Where(x => x.HasTeam(m.Home) && x.Date < m.Date).OrderByDescending(x => x.Date).Take(term).Select(x => Numbery.Normalise(x.GetOppositionStats(m.Home).Tackles, MatchStatistics.MIN_TACKLES, MatchStatistics.MAX_TACKLES, 0.0, 1.0)).ToList();
            for (var i = 0; i < ho.Count(); i++)
                ho.Insert(0, 0.0);
            var ao = matches.Where(x => x.HasTeam(m.Away) && x.Date < m.Date).OrderByDescending(x => x.Date).Take(term).Select(x => Numbery.Normalise(x.GetOppositionStats(m.Away).Tackles, MatchStatistics.MIN_TACKLES, MatchStatistics.MAX_TACKLES, 0.0, 1.0)).ToList();
            for (var i = 0; i < ao.Count(); i++)
                ao.Insert(0, 0.0);
            inputSet.AddRange(ht);
            inputSet.AddRange(at);
            inputSet.AddRange(ho);
            inputSet.AddRange(ao);
            return inputSet;
        }

        private IEnumerable<double> ExtractFreesInputSet(Match m, List<Match> matches, int term)
        {
            var inputSet = new List<double>();
            //Fors
            var htf = matches.Where(x => x.HasTeam(m.Home) && x.Date < m.Date).OrderByDescending(x => x.Date).Take(term).Select(x => Numbery.Normalise(x.GetTeamStats(m.Home).FreesFor, MatchStatistics.MIN_FREES, MatchStatistics.MAX_FREES, 0.0, 1.0)).ToList();
            for (var i = 0; i < htf.Count(); i++)
                htf.Insert(0, 0.0);
            var atf = matches.Where(x => x.HasTeam(m.Away) && x.Date < m.Date).OrderByDescending(x => x.Date).Take(term).Select(x => Numbery.Normalise(x.GetTeamStats(m.Away).FreesFor, MatchStatistics.MIN_FREES, MatchStatistics.MAX_FREES, 0.0, 1.0)).ToList();
            for (var i = 0; i < atf.Count(); i++)
                atf.Insert(0, 0.0);
            var hof = matches.Where(x => x.HasTeam(m.Home) && x.Date < m.Date).OrderByDescending(x => x.Date).Take(term).Select(x => Numbery.Normalise(x.GetOppositionStats(m.Home).FreesFor, MatchStatistics.MIN_FREES, MatchStatistics.MAX_FREES, 0.0, 1.0)).ToList();
            for (var i = 0; i < hof.Count(); i++)
                hof.Insert(0, 0.0);
            var aof = matches.Where(x => x.HasTeam(m.Away) && x.Date < m.Date).OrderByDescending(x => x.Date).Take(term).Select(x => Numbery.Normalise(x.GetOppositionStats(m.Away).FreesFor, MatchStatistics.MIN_FREES, MatchStatistics.MAX_FREES, 0.0, 1.0)).ToList();
            for (var i = 0; i < aof.Count(); i++)
                aof.Insert(0, 0.0);
            //Against
            var hta = matches.Where(x => x.HasTeam(m.Home) && x.Date < m.Date).OrderByDescending(x => x.Date).Take(term).Select(x => Numbery.Normalise(x.GetTeamStats(m.Home).FreesAgainst, MatchStatistics.MIN_FREES, MatchStatistics.MAX_FREES, 0.0, 1.0)).ToList();
            for (var i = 0; i < hta.Count(); i++)
                hta.Insert(0, 0.0);
            var ata = matches.Where(x => x.HasTeam(m.Away) && x.Date < m.Date).OrderByDescending(x => x.Date).Take(term).Select(x => Numbery.Normalise(x.GetTeamStats(m.Away).FreesAgainst, MatchStatistics.MIN_FREES, MatchStatistics.MAX_FREES, 0.0, 1.0)).ToList();
            for (var i = 0; i < ata.Count(); i++)
                ata.Insert(0, 0.0);
            var hoa = matches.Where(x => x.HasTeam(m.Home) && x.Date < m.Date).OrderByDescending(x => x.Date).Take(term).Select(x => Numbery.Normalise(x.GetOppositionStats(m.Home).FreesAgainst, MatchStatistics.MIN_FREES, MatchStatistics.MAX_FREES, 0.0, 1.0)).ToList();
            for (var i = 0; i < hoa.Count(); i++)
                hoa.Insert(0, 0.0);
            var aoa = matches.Where(x => x.HasTeam(m.Away) && x.Date < m.Date).OrderByDescending(x => x.Date).Take(term).Select(x => Numbery.Normalise(x.GetOppositionStats(m.Away).FreesAgainst, MatchStatistics.MIN_FREES, MatchStatistics.MAX_FREES, 0.0, 1.0)).ToList();
            for (var i = 0; i < aoa.Count(); i++)
                aoa.Insert(0, 0.0);
            inputSet.AddRange(htf);
            inputSet.AddRange(atf);
            inputSet.AddRange(hof);
            inputSet.AddRange(aof);
            inputSet.AddRange(hta);
            inputSet.AddRange(ata);
            inputSet.AddRange(hoa);
            inputSet.AddRange(aoa);
            return inputSet;
        }*/
        private IEnumerable<double> ExtractKicksInputSet(Match m, List<Match> matches, int term)
        {
            var inputSet = new List<double>();
            var ht = matches.Where(x => x.HasTeam(m.Home) && x.Date < m.Date).OrderByDescending(x => x.Date).Take(term).Select(x => Numbery.Normalise(x.GetTeamStats(m.Home).Kicks, MatchStatistics.MIN_KICKS, MatchStatistics.MAX_KICKS, 0.0, 1.0));
            var at = matches.Where(x => x.HasTeam(m.Away) && x.Date < m.Date).OrderByDescending(x => x.Date).Take(term).Select(x => Numbery.Normalise(x.GetTeamStats(m.Away).Kicks, MatchStatistics.MIN_KICKS, MatchStatistics.MAX_KICKS, 0.0, 1.0));
            var ho = matches.Where(x => x.HasTeam(m.Home) && x.Date < m.Date).OrderByDescending(x => x.Date).Take(term).Select(x => Numbery.Normalise(x.GetOppositionStats(m.Home).Kicks, MatchStatistics.MIN_KICKS, MatchStatistics.MAX_KICKS, 0.0, 1.0));
            var ao = matches.Where(x => x.HasTeam(m.Away) && x.Date < m.Date).OrderByDescending(x => x.Date).Take(term).Select(x => Numbery.Normalise(x.GetOppositionStats(m.Away).Kicks, MatchStatistics.MIN_KICKS, MatchStatistics.MAX_KICKS, 0.0, 1.0));
            inputSet.Add(ht.Count() == 0 ? 0.5 : ht.Average());
            inputSet.Add(at.Count() == 0 ? 0.5 : at.Average());
            inputSet.Add(ho.Count() == 0 ? 0.5 : ho.Average());
            inputSet.Add(ao.Count() == 0 ? 0.5 : ao.Average());
            return inputSet;
        }

        private IEnumerable<double> ExtractHandballsInputSet(Match m, List<Match> matches, int term)
        {
            var inputSet = new List<double>();
            var ht = matches.Where(x => x.HasTeam(m.Home) && x.Date < m.Date).OrderByDescending(x => x.Date).Take(term).Select(x => Numbery.Normalise(x.GetTeamStats(m.Home).Handballs, MatchStatistics.MIN_HANDBALLS, MatchStatistics.MAX_HANDBALLS, 0.0, 1.0));
            var at = matches.Where(x => x.HasTeam(m.Away) && x.Date < m.Date).OrderByDescending(x => x.Date).Take(term).Select(x => Numbery.Normalise(x.GetTeamStats(m.Away).Handballs, MatchStatistics.MIN_HANDBALLS, MatchStatistics.MAX_HANDBALLS, 0.0, 1.0));
            var ho = matches.Where(x => x.HasTeam(m.Home) && x.Date < m.Date).OrderByDescending(x => x.Date).Take(term).Select(x => Numbery.Normalise(x.GetOppositionStats(m.Home).Handballs, MatchStatistics.MIN_HANDBALLS, MatchStatistics.MAX_HANDBALLS, 0.0, 1.0));
            var ao = matches.Where(x => x.HasTeam(m.Away) && x.Date < m.Date).OrderByDescending(x => x.Date).Take(term).Select(x => Numbery.Normalise(x.GetOppositionStats(m.Away).Handballs, MatchStatistics.MIN_HANDBALLS, MatchStatistics.MAX_HANDBALLS, 0.0, 1.0));
            inputSet.Add(ht.Count() == 0 ? 0.5 : ht.Average());
            inputSet.Add(at.Count() == 0 ? 0.5 : at.Average());
            inputSet.Add(ho.Count() == 0 ? 0.5 : ho.Average());
            inputSet.Add(ao.Count() == 0 ? 0.5 : ao.Average());
            return inputSet;
        }

        private IEnumerable<double> ExtractMarksInputSet(Match m, List<Match> matches, int term)
        {
            var inputSet = new List<double>();
            var ht = matches.Where(x => x.HasTeam(m.Home) && x.Date < m.Date).OrderByDescending(x => x.Date).Take(term).Select(x => Numbery.Normalise(x.GetTeamStats(m.Home).Marks, MatchStatistics.MIN_MARKS, MatchStatistics.MAX_MARKS, 0.0, 1.0));
            var at = matches.Where(x => x.HasTeam(m.Away) && x.Date < m.Date).OrderByDescending(x => x.Date).Take(term).Select(x => Numbery.Normalise(x.GetTeamStats(m.Away).Marks, MatchStatistics.MIN_MARKS, MatchStatistics.MAX_MARKS, 0.0, 1.0));
            var ho = matches.Where(x => x.HasTeam(m.Home) && x.Date < m.Date).OrderByDescending(x => x.Date).Take(term).Select(x => Numbery.Normalise(x.GetOppositionStats(m.Home).Marks, MatchStatistics.MIN_MARKS, MatchStatistics.MAX_MARKS, 0.0, 1.0));
            var ao = matches.Where(x => x.HasTeam(m.Away) && x.Date < m.Date).OrderByDescending(x => x.Date).Take(term).Select(x => Numbery.Normalise(x.GetOppositionStats(m.Away).Marks, MatchStatistics.MIN_MARKS, MatchStatistics.MAX_MARKS, 0.0, 1.0));
            inputSet.Add(ht.Count() == 0 ? 0.5 : ht.Average());
            inputSet.Add(at.Count() == 0 ? 0.5 : at.Average());
            inputSet.Add(ho.Count() == 0 ? 0.5 : ho.Average());
            inputSet.Add(ao.Count() == 0 ? 0.5 : ao.Average());
            return inputSet;
        }

        private IEnumerable<double> ExtractHitoutsInputSet(Match m, List<Match> matches, int term)
        {
            var inputSet = new List<double>();
            var ht = matches.Where(x => x.HasTeam(m.Home) && x.Date < m.Date).OrderByDescending(x => x.Date).Take(term).Select(x => Numbery.Normalise(x.GetTeamStats(m.Home).HitOuts, MatchStatistics.MIN_HITOUTS, MatchStatistics.MAX_HITOUTS, 0.0, 1.0));
            var at = matches.Where(x => x.HasTeam(m.Away) && x.Date < m.Date).OrderByDescending(x => x.Date).Take(term).Select(x => Numbery.Normalise(x.GetTeamStats(m.Away).HitOuts, MatchStatistics.MIN_HITOUTS, MatchStatistics.MAX_HITOUTS, 0.0, 1.0));
            var ho = matches.Where(x => x.HasTeam(m.Home) && x.Date < m.Date).OrderByDescending(x => x.Date).Take(term).Select(x => Numbery.Normalise(x.GetOppositionStats(m.Home).HitOuts, MatchStatistics.MIN_HITOUTS, MatchStatistics.MAX_HITOUTS, 0.0, 1.0));
            var ao = matches.Where(x => x.HasTeam(m.Away) && x.Date < m.Date).OrderByDescending(x => x.Date).Take(term).Select(x => Numbery.Normalise(x.GetOppositionStats(m.Away).HitOuts, MatchStatistics.MIN_HITOUTS, MatchStatistics.MAX_HITOUTS, 0.0, 1.0));
            inputSet.Add(ht.Count() == 0 ? 0.5 : ht.Average());
            inputSet.Add(at.Count() == 0 ? 0.5 : at.Average());
            inputSet.Add(ho.Count() == 0 ? 0.5 : ho.Average());
            inputSet.Add(ao.Count() == 0 ? 0.5 : ao.Average());
            return inputSet;
        }

        private IEnumerable<double> ExtractTacklesInputSet(Match m, List<Match> matches, int term)
        {
            var inputSet = new List<double>();
            var ht = matches.Where(x => x.HasTeam(m.Home) && x.Date < m.Date).OrderByDescending(x => x.Date).Take(term).Select(x => Numbery.Normalise(x.GetTeamStats(m.Home).Tackles, MatchStatistics.MIN_TACKLES, MatchStatistics.MAX_TACKLES, 0.0, 1.0));
            var at = matches.Where(x => x.HasTeam(m.Away) && x.Date < m.Date).OrderByDescending(x => x.Date).Take(term).Select(x => Numbery.Normalise(x.GetTeamStats(m.Away).Tackles, MatchStatistics.MIN_TACKLES, MatchStatistics.MAX_TACKLES, 0.0, 1.0));
            var ho = matches.Where(x => x.HasTeam(m.Home) && x.Date < m.Date).OrderByDescending(x => x.Date).Take(term).Select(x => Numbery.Normalise(x.GetOppositionStats(m.Home).Tackles, MatchStatistics.MIN_TACKLES, MatchStatistics.MAX_TACKLES, 0.0, 1.0));
            var ao = matches.Where(x => x.HasTeam(m.Away) && x.Date < m.Date).OrderByDescending(x => x.Date).Take(term).Select(x => Numbery.Normalise(x.GetOppositionStats(m.Away).Tackles, MatchStatistics.MIN_TACKLES, MatchStatistics.MAX_TACKLES, 0.0, 1.0));
            inputSet.Add(ht.Count() == 0 ? 0.5 : ht.Average());
            inputSet.Add(at.Count() == 0 ? 0.5 : at.Average());
            inputSet.Add(ho.Count() == 0 ? 0.5 : ho.Average());
            inputSet.Add(ao.Count() == 0 ? 0.5 : ao.Average());
            return inputSet;
        }

        private IEnumerable<double> ExtractFreesInputSet(Match m, List<Match> matches, int term)
        {
            var inputSet = new List<double>();
            //Fors
            var htf = matches.Where(x => x.HasTeam(m.Home) && x.Date < m.Date).OrderByDescending(x => x.Date).Take(term).Select(x => Numbery.Normalise(x.GetTeamStats(m.Home).FreesFor, MatchStatistics.MIN_FREES, MatchStatistics.MAX_FREES, 0.0, 1.0));
            var atf = matches.Where(x => x.HasTeam(m.Away) && x.Date < m.Date).OrderByDescending(x => x.Date).Take(term).Select(x => Numbery.Normalise(x.GetTeamStats(m.Away).FreesFor, MatchStatistics.MIN_FREES, MatchStatistics.MAX_FREES, 0.0, 1.0));
            var hof = matches.Where(x => x.HasTeam(m.Home) && x.Date < m.Date).OrderByDescending(x => x.Date).Take(term).Select(x => Numbery.Normalise(x.GetOppositionStats(m.Home).FreesFor, MatchStatistics.MIN_FREES, MatchStatistics.MAX_FREES, 0.0, 1.0));
            var aof = matches.Where(x => x.HasTeam(m.Away) && x.Date < m.Date).OrderByDescending(x => x.Date).Take(term).Select(x => Numbery.Normalise(x.GetOppositionStats(m.Away).FreesFor, MatchStatistics.MIN_FREES, MatchStatistics.MAX_FREES, 0.0, 1.0));
            var hta = matches.Where(x => x.HasTeam(m.Home) && x.Date < m.Date).OrderByDescending(x => x.Date).Take(term).Select(x => Numbery.Normalise(x.GetTeamStats(m.Home).FreesAgainst, MatchStatistics.MIN_FREES, MatchStatistics.MAX_FREES, 0.0, 1.0));
            var ata = matches.Where(x => x.HasTeam(m.Away) && x.Date < m.Date).OrderByDescending(x => x.Date).Take(term).Select(x => Numbery.Normalise(x.GetTeamStats(m.Away).FreesAgainst, MatchStatistics.MIN_FREES, MatchStatistics.MAX_FREES, 0.0, 1.0));
            var hoa = matches.Where(x => x.HasTeam(m.Home) && x.Date < m.Date).OrderByDescending(x => x.Date).Take(term).Select(x => Numbery.Normalise(x.GetOppositionStats(m.Home).FreesAgainst, MatchStatistics.MIN_FREES, MatchStatistics.MAX_FREES, 0.0, 1.0));
            var aoa = matches.Where(x => x.HasTeam(m.Away) && x.Date < m.Date).OrderByDescending(x => x.Date).Take(term).Select(x => Numbery.Normalise(x.GetOppositionStats(m.Away).FreesAgainst, MatchStatistics.MIN_FREES, MatchStatistics.MAX_FREES, 0.0, 1.0));
            inputSet.Add(htf.Count() == 0 ? 0.5 : htf.Average());
            inputSet.Add(atf.Count() == 0 ? 0.5 : atf.Average());
            inputSet.Add(hof.Count() == 0 ? 0.5 : hof.Average());
            inputSet.Add(aof.Count() == 0 ? 0.5 : aof.Average());
            inputSet.Add(hta.Count() == 0 ? 0.5 : hta.Average());
            inputSet.Add(ata.Count() == 0 ? 0.5 : ata.Average());
            inputSet.Add(hoa.Count() == 0 ? 0.5 : hoa.Average());
            inputSet.Add(aoa.Count() == 0 ? 0.5 : aoa.Average());
            return inputSet;
        }

        protected abstract IEnumerable<double> ExtractInputSetForScore(Match m, List<Match> matches, int term,
            Func<Match, bool> homeWherePredicate, Func<Match, bool> awayWherePredicate);

        protected abstract IEnumerable<double> ExtractInputSetForWin(Match m, List<Match> matches, int term,
            Func<Match, bool> homeWherePredicate, Func<Match, bool> awayWherePredicate);


        protected abstract IEnumerable<double> ExtractInputSetForOppositionScore(int term,
            List<Tuple<Score, Score, DateTime>> homeOppositionScores,
            List<Tuple<Score, Score, DateTime>> awayOppositionScores);


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
            //TODO: confirm or remove
            //Attempting to reduce the impact of missing data by massaging 0's into 0.5s
            //return max == 0 ? 0.5 : Numbery.Normalise(value, max);
            return Numbery.Normalise(value, max);
        }

        public static double ExtractInputFromScoreScoreDateTuple(int takeLength,
    Func<Tuple<Score, Score, DateTime>, double> sumSelector, List<Tuple<Score, Score, DateTime>> homeOppositionScores, Func<double, double> maxFunc)
        {
            var value = homeOppositionScores
                .OrderByDescending(x => x.Item3)
                .Take(takeLength)
                .Sum(sumSelector);
            var max = maxFunc(
                homeOppositionScores
                    .OrderByDescending(x => x.Item3)
                    .Take(takeLength)
                    .Count());
            return Numbery.Normalise(value, max);
        }
        #endregion

        #region Outputs
        public abstract IEnumerable<double> BuildOutputs(Match m);
        #endregion

        #region constants
        public static double GetMaxSeasonRounds(double rounds)
        {
            return rounds;
        }

        public static double GetMaxSeasonTotal(double rounds)
        {
            return Util.MaxAverage * rounds;
        }

        public static double GetMaxSeasonGoals(double rounds)
        {
            return Util.MaxGoals * rounds;
        }

        public static double GetMaxSeasonPoints(double rounds)
        {
            return Util.MaxPoints * rounds;
        }
        #endregion
    }
}
