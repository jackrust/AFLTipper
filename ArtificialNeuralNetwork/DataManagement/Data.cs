using System;
using System.Collections.Generic;
using System.Linq;

namespace ArtificialNeuralNetwork.DataManagement
{
    public class Data
    {
        public List<DataPoint> DataPoints;
        //Todo, should this live here?
        public Func<List<double>, List<double>, string, bool> SuccessCondition;

        public Data()
        {
            DataPoints = new List<DataPoint>();
        }
        public List<List<double>> Inputs()
        {
            return DataPoints.Select(dp => dp.Inputs).ToList();
        }
        public List<List<double>> Outputs()
        {
            return DataPoints.Select(dp => dp.Outputs).ToList();
        }
    }

    public class DataPoint
    {
        public object Reference;
        public List<double> Inputs;
        public List<double> Outputs;

        public DataPoint()
        {
            Inputs = new List<double>();
            Outputs = new List<double>();
        }
        public DataPoint(List<double> input, List<double> output)
        {
            Inputs = input;
            Outputs = output;
        }
    }
}
