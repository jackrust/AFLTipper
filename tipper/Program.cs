using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;
using System.Net;
using Tipper.au.com.afl.xml;
using System.IO;
using ArtificialNeuralNetwork;
using Utilities;
using AustralianRulesFootball;

namespace Tipper
{
    class Program
    {
        //public static Tipper tipper = new Tipper();
        static void Main(string[] args)
        {
            Console.WriteLine("Start");
            Tipper tipper = new Tipper();
            Data trainingData = tipper.LearnFromTo(2012, 24 - Tipper.RELEVANT_ROUND_HISTORY, 2013, 24);
            Data testingData = tipper.LearnFromTo(2013, 24 - Tipper.RELEVANT_ROUND_HISTORY, 2014, 24);

            var optimizer = new ArtificialNeuralNetwork.Optimizer();
            optimizer.Optimize(trainingData, testingData, SuccessCondition);

            Console.Read();
        }

        public static bool SuccessCondition(List<double> predicted, List<double> actual)
        {
            var phGoals = Numbery.Denormalise(predicted[0], Util.MAX_GOALS);
            var phPoints = Numbery.Denormalise(predicted[1], Util.MAX_POINTS);
            var paGoals = Numbery.Denormalise(predicted[2], Util.MAX_GOALS);
            var paPoints = Numbery.Denormalise(predicted[3], Util.MAX_POINTS);
            var phScore = phGoals * 6 + phPoints;
            var paScore = paGoals * 6 + paPoints;

            var ahGoals = Numbery.Denormalise(actual[0], Util.MAX_GOALS);
            var ahPoints = Numbery.Denormalise(actual[1], Util.MAX_POINTS);
            var aaGoals = Numbery.Denormalise(actual[2], Util.MAX_GOALS);
            var aaPoints = Numbery.Denormalise(actual[3], Util.MAX_POINTS);
            var ahScore = ahGoals * 6 + ahPoints;
            var aaScore = aaGoals * 6 + aaPoints;

            if (phScore > paScore && ahScore > aaScore)
                return true;
            if (phScore < paScore && ahScore < aaScore)
                return true;
            return false;
        }
    }
}
