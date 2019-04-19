using System;

namespace ArtificialNeuralNetwork
{
    [Serializable]
    public class Bias : Input {
        public static double DefaultValue = -1;
        public static String DefaultName = "B";

	    public Bias():base(DefaultName, DefaultValue) {}
    }
}
