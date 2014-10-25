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
            Data data = tipper.LoadData();
            List<List<double>> trainingInputs = new List<List<double>>();
            List<List<double>> trainingOutputs = new List<List<double>>();
            List<List<double>> testingInputs = new List<List<double>>();
            List<List<double>> testingOutputs = new List<List<double>>();
            var optimizer = new ArtificialNeuralNetwork.Optimizer();
            optimizer.Optimize(trainingInputs, trainingOutputs, testingInputs, testingOutputs);

            Console.Read();
        }  
    }
}
