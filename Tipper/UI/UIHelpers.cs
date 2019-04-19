using System;
using System.Collections.Generic;
using AustralianRulesFootball;
using Utilities;

namespace Tipper.UI
{
    public class UIHelpers
    {

        #region Helpers
        public static bool SuccessConditionGoalAndPoints(List<double> predicted, List<double> actual)
        {
            var phGoals = Numbery.Denormalise(predicted[0], Util.MaxGoals);
            var phPoints = Numbery.Denormalise(predicted[1], Util.MaxPoints);
            var paGoals = Numbery.Denormalise(predicted[2], Util.MaxGoals);
            var paPoints = Numbery.Denormalise(predicted[3], Util.MaxPoints);
            var phScore = phGoals * 6 + phPoints;
            var paScore = paGoals * 6 + paPoints;

            var ahGoals = Numbery.Denormalise(actual[0], Util.MaxGoals);
            var ahPoints = Numbery.Denormalise(actual[1], Util.MaxPoints);
            var aaGoals = Numbery.Denormalise(actual[2], Util.MaxGoals);
            var aaPoints = Numbery.Denormalise(actual[3], Util.MaxPoints);
            var ahScore = ahGoals * 6 + ahPoints;
            var aaScore = aaGoals * 6 + aaPoints;

            //Console.WriteLine("[{0}, {1} Vs {2}, {3}]", phScore, paScore, ahScore, aaScore);

            if (phScore > paScore && ahScore > aaScore)
                return true;
            if (phScore < paScore && ahScore < aaScore)
                return true;
            if (phScore == paScore && ahScore == aaScore)
                return true;
            return false;
        }

        public static bool SuccessConditionGoalAndPointsPrint(List<double> predicted, List<double> actual, string print)
        {
            Func<double, double> rule = (m => m > 27.0 ? 15.00 : 0.00);

            var phGoals = Numbery.Denormalise(predicted[0], Util.MaxGoals);
            var phPoints = Numbery.Denormalise(predicted[1], Util.MaxPoints);
            var paGoals = Numbery.Denormalise(predicted[2], Util.MaxGoals);
            var paPoints = Numbery.Denormalise(predicted[3], Util.MaxPoints);
            var phScore = phGoals * 6 + phPoints;
            var paScore = paGoals * 6 + paPoints;

            var ahGoals = Numbery.Denormalise(actual[0], Util.MaxGoals);
            var ahPoints = Numbery.Denormalise(actual[1], Util.MaxPoints);
            var aaGoals = Numbery.Denormalise(actual[2], Util.MaxGoals);
            var aaPoints = Numbery.Denormalise(actual[3], Util.MaxPoints);
            var ahScore = ahGoals * 6 + ahPoints;
            var aaScore = aaGoals * 6 + aaPoints;

            var margin = Math.Abs(phScore - ahScore);
            var wager = rule(margin);

            if (print == "")
                Console.WriteLine("[{0}, {1} Vs {2}, {3}] Suggested Bet: ${4:0.00}", phScore, paScore, ahScore, aaScore, wager);
            else if (!string.IsNullOrEmpty(print))
                Filey.Save(string.Format("\n[{0}, {1} Vs {2}, {3}] Suggested Bet: ${4:0.00}", phScore, paScore, ahScore, aaScore, wager), print);

            if (phScore > paScore && ahScore > aaScore)
                return true;
            if (phScore < paScore && ahScore < aaScore)
                return true;
            if (phScore == paScore && ahScore == aaScore)
                return true;
            return false;
        }

        public static bool SuccessConditionLadderPrint(List<double> predicted, List<double> actual, string print)
        {
            var phLadder = Numbery.Denormalise(predicted[0], Util.MaxLadderPoints);
            var paLadder = Numbery.Denormalise(predicted[1], Util.MaxLadderPoints);

            var ahLadder = Numbery.Denormalise(actual[0], Util.MaxLadderPoints);
            var aaLadder = Numbery.Denormalise(actual[1], Util.MaxLadderPoints);

            if (print == "")
                Console.WriteLine("[{0}, {1} Vs {2}, {3}] Suggested Bet: ${8:0.00}", phLadder, paLadder, ahLadder, aaLadder);
            else if (!string.IsNullOrEmpty(print))
                Filey.Save(string.Format("[{0}, {1} Vs {2}, {3}] Suggested Bet: ${8:0.00}", phLadder, paLadder, ahLadder, aaLadder), print);

            if (phLadder > paLadder && ahLadder > aaLadder)
                return true;
            if (phLadder < paLadder && ahLadder < aaLadder)
                return true;
            if (phLadder == paLadder && ahLadder == aaLadder)
                return true;
            return false;
        }

        public static bool SuccessConditionTotalPrint(List<double> predicted, List<double> actual, string print = "")
        {
            var phTotal = Numbery.Denormalise(predicted[0], Util.MaxScore);
            var paTotal = Numbery.Denormalise(predicted[1], Util.MaxScore);

            var ahTotal = Numbery.Denormalise(actual[0], Util.MaxScore);
            var aaTotal = Numbery.Denormalise(actual[1], Util.MaxScore);

            if (print == "")
                Console.WriteLine("[{0}, {1} Vs {2}, {3}]", phTotal, paTotal, ahTotal, aaTotal);
            else if (!string.IsNullOrEmpty(print))
                Filey.Append(string.Format("[{0}, {1} Vs {2}, {3}]", phTotal, paTotal, ahTotal, aaTotal), print);

            if (phTotal > paTotal && ahTotal > aaTotal)
                return true;
            if (phTotal < paTotal && ahTotal < aaTotal)
                return true;
            if (phTotal == paTotal && ahTotal == aaTotal)
                return true;
            return false;
        }

        public static bool SuccessConditionTotal(List<double> predicted, List<double> actual)
        {
            var phTotal = Numbery.Denormalise(predicted[0], Util.MaxScore);
            var paTotal = Numbery.Denormalise(predicted[1], Util.MaxScore);

            var ahTotal = Numbery.Denormalise(actual[0], Util.MaxScore);
            var aaTotal = Numbery.Denormalise(actual[1], Util.MaxScore);

            Filey.Append(string.Format("[{0}, {1} Vs {2}, {3}]", phTotal, paTotal, ahTotal, aaTotal), @"C:\Users\Jack\Desktop\ANN Optimizer results.txt");

            if (phTotal > paTotal && ahTotal > aaTotal)
                return true;
            if (phTotal < paTotal && ahTotal < aaTotal)
                return true;
            if (phTotal == paTotal && ahTotal == aaTotal)
                return true;
            return false;
        }

        public static bool SuccessConditionTotalAsymptotic(List<double> predicted, List<double> actual, string legacy)
        {
            var phTotal = Numbery.Denormalise(predicted[0], Util.MaxScore, Numbery.NormalisationMethod.Asymptotic);
            var paTotal = Numbery.Denormalise(predicted[1], Util.MaxScore, Numbery.NormalisationMethod.Asymptotic);

            var ahTotal = Numbery.Denormalise(actual[0], Util.MaxScore);
            var aaTotal = Numbery.Denormalise(actual[1], Util.MaxScore);


            if (phTotal > paTotal && ahTotal > aaTotal)
                return true;
            if (phTotal < paTotal && ahTotal < aaTotal)
                return true;
            if (phTotal == paTotal && ahTotal == aaTotal)
                return true;
            return false;
        }

        public static bool SuccessConditionLadderGoalsAndPointsPrint(List<double> predicted, List<double> actual, string print)
        {
            Func<double, double> rule = (m => m > 27.0 ? 15.00 : 0.00);

            var phLadder = Numbery.Denormalise(predicted[0], Util.MaxLadderPoints);
            var paLadder = Numbery.Denormalise(predicted[1], Util.MaxLadderPoints);
            var phGoals = Numbery.Denormalise(predicted[2], Util.MaxGoals);
            var phPoints = Numbery.Denormalise(predicted[3], Util.MaxPoints);
            var paGoals = Numbery.Denormalise(predicted[4], Util.MaxGoals);
            var paPoints = Numbery.Denormalise(predicted[5], Util.MaxPoints);
            var phScore = phGoals * 6 + phPoints;
            var paScore = paGoals * 6 + paPoints;

            var ahLadder = Numbery.Denormalise(actual[0], Util.MaxLadderPoints);
            var aaLadder = Numbery.Denormalise(actual[1], Util.MaxLadderPoints);
            var ahGoals = Numbery.Denormalise(actual[2], Util.MaxGoals);
            var ahPoints = Numbery.Denormalise(actual[3], Util.MaxPoints);
            var aaGoals = Numbery.Denormalise(actual[4], Util.MaxGoals);
            var aaPoints = Numbery.Denormalise(actual[5], Util.MaxPoints);
            var ahScore = ahGoals * 6 + ahPoints;
            var aaScore = aaGoals * 6 + aaPoints;

            var margin = Math.Abs(phScore - ahScore);
            var wager = rule(margin);

            if (print == "")
                Console.WriteLine("[{0}: {1}, {2}: {3} Vs {4}: {5}, {6}: {7}] Suggested Bet: ${8:0.00}", phLadder, phScore, paLadder, paScore, ahLadder, ahScore, aaLadder, aaScore, wager);
            else if (!string.IsNullOrEmpty(print))
                Filey.Save(string.Format("[{0}: {1}, {2}: {3} Vs {4}: {5}, {6}: {7}] Suggested Bet: ${8:0.00}", phLadder, phScore, paLadder, paScore, ahLadder, ahScore, aaLadder, aaScore, wager), print);

            if (phScore > paScore && ahScore > aaScore)
                return true;
            if (phScore < paScore && ahScore < aaScore)
                return true;
            if (phScore == paScore && ahScore == aaScore)
                return true;
            return false;
        }

        public static double BetReturnGoalAndPoints(List<double> predicted, List<double> actual, double homeOdds, double awayOdds)
        {
            var phGoals = Numbery.Denormalise(predicted[0], Util.MaxGoals);
            var phPoints = Numbery.Denormalise(predicted[1], Util.MaxPoints);
            var paGoals = Numbery.Denormalise(predicted[2], Util.MaxGoals);
            var paPoints = Numbery.Denormalise(predicted[3], Util.MaxPoints);
            var phScore = phGoals * 6 + phPoints;
            var paScore = paGoals * 6 + paPoints;

            var ahGoals = Numbery.Denormalise(actual[0], Util.MaxGoals);
            var ahPoints = Numbery.Denormalise(actual[1], Util.MaxPoints);
            var aaGoals = Numbery.Denormalise(actual[2], Util.MaxGoals);
            var aaPoints = Numbery.Denormalise(actual[3], Util.MaxPoints);
            var ahScore = ahGoals * 6 + ahPoints;
            var aaScore = aaGoals * 6 + aaPoints;

            //Console.WriteLine("[{0}, {1} Vs {2}, {3}]", phScore, paScore, ahScore, aaScore);

            if (phScore > paScore && ahScore > aaScore)
                return homeOdds;
            if (phScore < paScore && ahScore < aaScore)
                return awayOdds;
            if (phScore == paScore && ahScore == aaScore)
                return 1.00;
            return 0.00;
        }

        public static List<double> Deconvert(List<double> input)
        {
            var hGoals = Numbery.Denormalise(input[0], Util.MaxGoals);
            var hPoints = Numbery.Denormalise(input[1], Util.MaxPoints);
            var aGoals = Numbery.Denormalise(input[2], Util.MaxGoals);
            var aPoints = Numbery.Denormalise(input[3], Util.MaxPoints);
            return new List<double>(){
                hGoals, hPoints, aGoals, aPoints
            };
        }

        public static bool SuccessConditionScore(List<double> predicted, List<double> actual)
        {
            var phScore = Numbery.Denormalise(predicted[0], Util.MaxScore);
            var paScore = Numbery.Denormalise(predicted[1], Util.MaxScore);

            var ahScore = Numbery.Denormalise(actual[0], Util.MaxScore);
            var aaScore = Numbery.Denormalise(actual[1], Util.MaxScore);

            if (phScore > paScore && ahScore > aaScore)
                return true;
            if (phScore < paScore && ahScore < aaScore)
                return true;
            if (phScore == paScore && ahScore == aaScore)
                return true;
            return false;
        }

        public static bool SuccessConditionWinLoss(List<double> predicted, List<double> actual)
        {

            if (predicted[0] > 0.5 && actual[0] > 0.5)
                return true;
            if (predicted[0] < 0.5 && actual[0] < 0.5)
                return true;
            if (predicted[0] == 0.5 && actual[0] == 0.5)
                return true;
            return false;
        }
        #endregion
    }
}
