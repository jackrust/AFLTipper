using System;
using System.Collections.Generic;
using System.Linq;

namespace ArtificialNeuralNetwork
{
    [Serializable]
    public abstract class Neuron
    {

        public static double DefaultThreshold = 0.5;
        public static String DefaultName = "N";
        public double Threshold = DefaultThreshold;
        public String Name = DefaultName;
        public List<Dendrite> Dendrites;

        protected Neuron(){}

        /**
         * Extended Constructor
         * @param inputs Vector<> of input INeurons
         */

        protected Neuron(IEnumerable<Neuron> inputs):this()
        {
            foreach (var i in inputs)
                Dendrites.Add(new Dendrite(i));
        }

        /**
         * Extended Constructor
         * @param name (String)
         */

        protected Neuron(String name)
            : this(new List<Dendrite>(), name, DefaultThreshold)
        {
        }

        /**
         * Full Constructor
         * @param dendrites Vector<> of input strands
         * @param threshold 
         * @param value 
         * @param name String identifier
         */

        protected Neuron(List<Dendrite> dendrites, String name, double threshold)
        {
            Dendrites = dendrites;
            Name = name;
            Threshold = threshold;
            if (name != "B" && dendrites.All(d => d.Neuron.Name != "B"))
                Dendrites.Add(new Dendrite(new Bias()));
        }

        /**
         * getOutput
         * Abstract, all nodes must return an output
         * @return double value
         */
        public abstract double GetOutput(NeuralPathway path = null);

        /**
         * backpropagate
         * all nodes must backpropagate errors
         * @param error (double)
         */
        public abstract void Backpropagate(double error, NeuralPathway path = null);

        public static Neuron[][] Copy(List<List<Neuron>> input)
        {
            var output = new Neuron[input.Count()][];
            for (var i = 0; i < input.Count(); i++ )
            {
                output[i] = Copy(input[i]).ToArray();
            }
            return output;
        }

        public static List<Neuron> Copy(List<Neuron> input)
        {
            var output = new List<Neuron>();
            foreach (var i in input)
            {
                if (i.Name.ToUpper().Contains("I"))
                    output.Add(Input.Copy((Input)i));
                else
                    output.Add(Dynamic.Copy((Dynamic)i));
            }
            return output;
        }

        protected bool Equivelant(Neuron other)
        {
            return other.Name == Name;
        }

        // Getters & Setters
        public void AddDendrite(Neuron dendrite) 
        {
            Dendrites.Add(new Dendrite(dendrite));
        }
        public void AddDendrite(Neuron dendrite, double weight) 
        {
            Dendrites.Add(new Dendrite(dendrite, weight)); 
        }
        public void AddDendrites(List<Neuron> dendrites)
        {
            foreach (var d in dendrites.Where(d => !d.Equivelant(this)))
            {
                AddDendrite(d);
            }
        }

        public List<double> GetWeights()
        {
            return Dendrites.Select(d => d.Weight).ToList();
        }

        public void SetWeights(List<double> weights)
        {
            if (weights.Count != Dendrites.Count)
                return;
            for (var i = 0; i < weights.Count; i++)
            {
                Dendrites[i].Weight = weights[i];
            }
        }

        public IEnumerable<NeuralPathway> ExtendPathway(NeuralPathway pathway)
        {
            var pathways = new List<NeuralPathway>();
            if (Dendrites.Count <= 0)
                return new List<NeuralPathway>() {pathway};
            foreach (var d in Dendrites)
            {
                var path = pathway.Copy();
                path.Path.Add(d.Neuron);
                path.Weightings.Add(d.Weight);
                var paths = d.Neuron.ExtendPathway(path);
                pathways.AddRange(paths);
            }
            return pathways;
        }
    }
}
