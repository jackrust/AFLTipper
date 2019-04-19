using System;
using System.Collections.Generic;
using System.Linq;

namespace ArtificialNeuralNetwork
{
    [Serializable]
    public class Input:Neuron
    {
        public static double DefaultValue = 1;
        public double Value = DefaultValue;


        public Input() { Dendrites = new List<Dendrite>(); }


        public Input(String name)
            : this(name, DefaultValue)
        {
            Dendrites = new List<Dendrite>();
	    }

	    public Input(double value):this(DefaultName, value)
        {
            Dendrites = new List<Dendrite>();
	    }
	
	    public Input(String name, double value):base(name)
        {
		    Value = value;
            Dendrites = new List<Dendrite>();
	    }

        public Input(List<Dendrite> dendrites, String name, double threshold, double value)
            : base(dendrites, name, threshold)
        {
            Value = value;
            Dendrites = new List<Dendrite>();
        }
	
	    /**
	     * getOutput
	     * Static INeurons return their value
	     * @return double value
	     */
        public override double GetOutput(NeuralPathway path = null)
        {
		    return Value;
	    }
	
	    /**
	     * backpropagate
	     * Static neurons do not need to backpropagate
	     */
	    public override void Backpropagate(double error, NeuralPathway path = null) {}

        public static List<List<Input>> Copy(List<List<Input>> input)
        {
            return input.Select(Copy).ToList();
        }

        public static List<Input> Copy(List<Input> input)
        {
            return input.Select(Copy).ToList();
        }

        public static Input Copy(Input input){
            return new Input(Dendrite.Copy(input.Dendrites), input.Name, input.Threshold, input.Value);
        }

        public new void AddDendrites(List<Neuron> dendrites)
        {
            //Inputs do not have dendrites
        }

    }
}
