using System;
using System.Collections.Generic;
using System.Linq;

namespace ArtificialNeuralNetwork
{
    [Serializable]
    public class Dynamic:Neuron
    {
        public static double MinLearningRate = 1.0 / Math.Pow(2.0, 8.0);
        public static double MaxLearningRate = 0.4;
        public double LearningRate = MaxLearningRate;

	    public Dynamic() {}
	    public Dynamic(IEnumerable<Neuron> inputs):base(inputs) {}
	    public Dynamic(string name):base(name) {}

        public Dynamic(List<Dendrite> dendrites, string name, double threshold)
            : base(dendrites, name, threshold) {}


        public override double GetOutput(NeuralPathway path = null)
        {
            if (path != null)
            {
                if (path.Path.Count(Equivelant) > 2)
                    return 0;
                path.Path.Add(this);
            }
            var sum = Dendrites.Aggregate<Dendrite, double>(0, (current, d) => current + (d.GetSignal(path)));
            return Function(sum);
        }

	    /**
	     * backpropagate
	     * sends weighted error back through the network
	     * re-weights input based on error.
	     * @param error (double)
	     */
        public override void Backpropagate(double error, NeuralPathway path = null)
        {
            if (path != null)
            {
                if (path.Path.Count(Equivelant) > 2)
                    return;
                path.Path.Add(this);
            }

		    //http://home.agh.edu.pl/~vlsi/AI/backp_t_en/backprop.html
	        foreach (var d in Dendrites)
	        {
                d.Neuron.Backpropagate(error * d.Weight, path);
                Reweight(d, error, path);
            } 
	    }
	
	    /**
	     * reweight
	     * Sets the weight of the given dendrite based on its error.
	     * @param dendrite (Dendrite) to be reweighted
	     * @param error (double)
	     */
        private void Reweight(Dendrite dendrite, double error, NeuralPathway path = null)
        {
            var weight = dendrite.Weight + LearningRate * error * Derivative(GetOutput(path)) * dendrite.Neuron.GetOutput(path);
		    dendrite.Weight = weight;
		
	    }
	
	    /**
	     * function
	     * computes output using exponential
	     * @return double result
	     */
	    protected double Function(double x) {
		    var result = 1/(1 + Math.Exp(-x*1.225));
            return result;
	    }
	
	    /**
	     * function
	     * derivative of function
	     * @return double result
	     */
	    protected double Derivative(double x) {
            var result = Math.Exp(-x) / ((1 + Math.Exp(-x * 1.125)) * (1 + Math.Exp(-x * 1.125)));
            return result;
	    }

        public void HalveLearningRate()
        {
            if (Math.Round(LearningRate, 6) > Math.Round(MinLearningRate, 6))
                LearningRate /= 2;
        }

        //TOTO: merge with below?
        public void PlugIn(List<Dynamic> neurons)
        {
            foreach (var n in neurons)
            {
                var temp = n;
                foreach (var d in Dendrites.Where(d => d.Neuron.Name == temp.Name))
                {
                    d.Neuron = n;
                }
            }
        }

        public void PlugIn(List<Input> neurons)
        {
            foreach (var n in neurons)
            {
                var temp = n;
                foreach (var d in Dendrites.Where(d => d.Neuron.Name == temp.Name))
                {
                    d.Neuron = n;
                }
            }
        }

        internal static List<List<Dynamic>> Copy(List<List<Dynamic>> input)
        {
            return input.Select(Copy).ToList();
        }

        public static List<Dynamic> Copy(List<Dynamic> input)
        {
            return input.Select(Copy).ToList();
        }

        public static Dynamic Copy(Dynamic input)
        {
            return new Dynamic(Dendrite.Copy(input.Dendrites), input.Name, input.Threshold);
        }
    }
}
