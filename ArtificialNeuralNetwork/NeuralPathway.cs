using System.Collections.Generic;
using System.Linq;

namespace ArtificialNeuralNetwork
{
    public class NeuralPathway
    {
        public List<Neuron> Path;
        public List<double> Weightings;

        public NeuralPathway()
        {
            Path = new List<Neuron>();
            Weightings = new List<double>();
        }


        public NeuralPathway Copy()
        {
            var copy = new NeuralPathway();
            foreach (var p in Path)
            {
                copy.Path.Add(p);
            }
            foreach (var w in Weightings)
            {
                copy.Weightings.Add(w);
            }
            return copy;
        }

        internal double WeightingProduct()
        {
            return Weightings.Aggregate(1.0, (current, w) => current*w);
        }
    }
}
