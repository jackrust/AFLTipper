using System;
using System.Collections.Generic;
using System.Linq;

namespace ArtificialNeuralNetwork
{
    [Serializable]
    public class Dendrite
    {
        public static Random Random = new Random();
        public static int RandomUpper = 1;
        public Neuron Neuron;
        public double Weight;

        public Dendrite() : this(new Bias(), Random.Next(RandomUpper)) {}

        public Dendrite(Neuron neuron) : this(neuron, Random.Next(RandomUpper)) {}

        public Dendrite(Neuron neuron, double weight)
        {
            Neuron = neuron;
            Weight = weight;
        }

        public double GetSignal(NeuralPathway path = null)
        {
            return GetWeightedOutput(path);
        }
        public double GetWeightedOutput(NeuralPathway path = null)
        {
            return Weight * Neuron.GetOutput(path);
        }

        public static List<Dendrite> Copy(List<Dendrite> input)
        {
            return input.Select(Copy).ToList();
        }

        public static Dendrite Copy(Dendrite input)
        {
            Neuron neuron;
            var inputNeuron= input.Neuron as Input;
            if (inputNeuron != null)
                neuron = Input.Copy(inputNeuron);
            else
                neuron = Dynamic.Copy((Dynamic)input.Neuron);
            Dendrite output = new Dendrite(neuron, input.Weight);
            return output;
        }
    }
}
