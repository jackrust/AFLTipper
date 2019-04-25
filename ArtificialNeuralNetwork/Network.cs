using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Web.Helpers;
using ArtificialNeuralNetwork.DataManagement;

namespace ArtificialNeuralNetwork
{
    [Serializable]
    public class Network
    {
        public string Id = Guid.NewGuid().ToString();
        public string Details = "";

        public static int DefaultMaxEpochs = 1500;
        public static int DefaultMaxMinima = 50;
        public static double DefaultTargetError = 0.005;
        public static string DefaultDirectory = "Network/";

        public int MaxEpochs = DefaultMaxEpochs;
        public int MaxMinima = DefaultMaxMinima;
        public double TargetError = DefaultTargetError;
        public string Directory = DefaultDirectory;
        public int Epochs;
        public double Error;

        public List<Input> INeurons;
        public List<List<Dynamic>> HLayers;
        public List<Dynamic> ONeurons;


        public Network()
        {
            INeurons = new List<Input>();
            HLayers = new List<List<Dynamic>>();
            ONeurons = new List<Dynamic>();
        }
        public Network(int inputsNo, IReadOnlyCollection<int> hiddenNos, int outputsNo)
        {
            CreateInputs(inputsNo);
            CreateHiddenLayers(hiddenNos.Count);
            var layer = 1;
            foreach (var h in hiddenNos)
            {
                CreateHiddenLayer(layer, h);
                layer++;
            }
            CreateOutputs(outputsNo);
        }

        public static Network CreateNetwork(Data trainingData, int numLayers, int perLayer, TrainingAlgorithmFactory.TrainingAlgorithmType algorithm)
        {
            //Create hidden layers
            var hidden = new List<int>();

            for (var i = 0; i < numLayers; i++)
            {
                hidden.Add(perLayer);
            }

            //Create Network
            var network = new Network(trainingData.DataPoints[0].Inputs.Count, hidden, trainingData.DataPoints[0].Outputs.Count);
            //New network with 5 inputs, One hidden layer of 2 neurons, 1 output

            //TODO: Training should be a seperate call <- this might explain the over fitting aswell
            //Train the network
            //network.Train(trainingData.Inputs(), trainingData.Outputs(), algorithm);

            return network;
        }

        /**
	     * run
	     * Returns the outputs from the given inputs
	     * @param inputs, a List<double> of input values
	     * @return outputs, a List<double> of output values from 0-1
	     */
	    public List<double> Run(List<double> inputs) {
		    //If inputs and/or outputs aren't set up then bail now
            if (inputs.Count != INeurons.Count || !(ONeurons.Count > 0))
			    return null;
		
		    //Load input values into the input neurons
		    for(var i = 0; i < inputs.Count; i++)
			    INeurons[i].Value = inputs[i];
		
		    return Run();
	    }
	
	    /**
	     * run
	     * Returns the outputs from the stored inputs
	     * @return outputs, a List<double> of output values from 0-1
	     */
        protected List<double> Run()
        {
            //If inputs and/or outputs aren't set up then bail now
            if (!(INeurons.Count > 0) || !(ONeurons.Count > 0))
                return null;

            //Pull the output from the output neurons
            return ONeurons.Select(o => o.GetOutput()).ToList();
        }
	
	    /**
	     * train
	     * trains the network from given inputs and target outputs
	     * @param inputs, List<List<double>> a list of input Lists
	     * @param targets, List<List<double>> a list of corresponding target Lists
	     */
        public void Train(List<List<double>> inputs, List<List<double>> targets, TrainingAlgorithmFactory.TrainingAlgorithmType trainingAlgorithmType = TrainingAlgorithmFactory.TrainingAlgorithmType.HoldBestInvestigate)
        {
            var algorithm = TrainingAlgorithmFactory.CreateAlgoRithm(trainingAlgorithmType);
            algorithm.Train(this, inputs, targets);
        }


        public double TrainEpoch(IReadOnlyList<List<double>> inputs, IReadOnlyList<List<double>> targets)
        {
            Error = 0;
            //for each of the training cases
            for (var i = 0; i < inputs.Count; i++)
            {
                //get the output List for the given input List
                var outputs = Run(inputs[i]);

                //For each of the output values
                for (var j = 0; j < outputs.Count; j++)
                {
                    //Find the error
                    var err = targets[i][j] - outputs[j];

                    //And backpropagate to train the network
                    ONeurons[j].Backpropagate(err);
                    
                    //Store error for checking
                    Error = Error + Math.Abs(err);
                }
            }
            return Error;
        }
        
        #region Create
	    /**
	     * createInputs
	     * sets up the input layer
	     * @param num, number of inputs
	     * @return true if the create was successful
	     */
	    public bool CreateInputs(int num){
            INeurons = new List<Input>();
            for (int i = 0; i < num; i++)
			    INeurons.Add(new Input("I"+(i+1), 0));
		
		    return true;
	    }

	    /**
	     * createHiddenLayers
	     * sets up the input layer
	     * @param num, number of layers
	     * @return true if the create was successful
	     */
	    public bool CreateHiddenLayers(int num) {
            HLayers = new List<List<Dynamic>>();
		    for(int i = 0; i < num; i++)
                HLayers.Add(new List<Dynamic>());
		    return true;
	    }
	
	    /**
	     * createHiddenLayer
	     * sets up the specified layer with 'num' neurons
	     * @param layer, the hidden layer to set from 1-n
	     * @param num, number of hidden neurons in the layer
	     * @return true if the create was successful
	     * @prereq createInputs, createHiddenLayers
	     */
	    public bool CreateHiddenLayer(int layer, int num) {
            if (!(INeurons.Count > 0) || layer < 1 || num < 1)
			    return false;
            HLayers[layer - 1] = new List<Dynamic>();
		    //add 'num' neurons to hidden layer 'layer'
		    for(int i = 0; i < num; i++)
			    HLayers[layer-1].Add(new Dynamic("H"+(layer-1)+"."+(i+1)));

		    //if this is the first hidden layer connect to input neurons
            if (layer == 1)
            {
                foreach (var h in HLayers[layer - 1])
                    foreach (var i in INeurons)
                        h.AddDendrite(i);
            }
		    //if this is the second+ layer, connect to previous hidden neurons
            if (layer > 1)
            {
                foreach (var h in HLayers[layer - 1])
                    foreach (var p in HLayers[layer - 2])
                        h.AddDendrite(p);
            }
		    return true;
	    }
	
	    /**
	     * createOutputs
	     * sets up the output layer with 'num' neurons
	     * @param layer, the hidden layer to set from 1-n
	     * @param num, number of hidden neurons in the layer
	     * @return true if the create was successful
	     * @prereq createInputs, createHiddenLayers, createHiddenLayer
	     */
	    public bool CreateOutputs(int num){
            if (!(INeurons.Count > 0) || !HiddenLayersValid())
			    return false;
		
            ONeurons = new List<Dynamic>();
		
		    //Create Layer
		    for(int i = 0; i < num; i++)
			    ONeurons.Add(new Dynamic("O"+(i+1)));
		
		    //if there is at least one hidden layer, hook up to last one
            if (HiddenLayersExist())
            {
                foreach (var o in ONeurons)
                    foreach (var h in HLayers[HLayers.Count() - 1])
                        o.AddDendrite(h);
            }
            //else hook up to the input layer
            else
            {
                foreach (var o in ONeurons)
                    foreach (var i in INeurons)
                        o.AddDendrite(i);
            }
		    return true;
	    }
        #endregion

	    #region Validate
	    /**
	     * hiddenLayersExist
	     * @return true if hidden layers exist
	     */
        protected bool HiddenLayersExist() { return (HLayers.Any()); }

	    /**
	     * hiddenLayersValid
	     * @return true if hidden layers are sound
	     */
        protected bool HiddenLayersValid()
	    {
	        return HLayers.All(layer => layer.All(neuron => neuron != null));
	    }
        #endregion

        #region Display
	    /**
	     * printNetwork
	     * converts the Network weights to a string
	     * @return human friendly string
	     */
	    public String Print()
	    {
		    //Set names (in case not already set)
		    String print = "";
		    for(int i = 0; i < INeurons.Count(); i++)
			    INeurons[i].Name = "I"+(i+1);
		    for(int i = 0; i < HLayers.Count(); i++)
			    for(int j = 0; j < HLayers[i].Count(); j++)
				    HLayers[i][j].Name = "H"+(i+1)+"."+(j+1);
		    for(int i = 0; i < ONeurons.Count(); i++)
			    ONeurons[i].Name ="O"+(i+1);
		
		    //Input layer has not connections
		    print = print + "Inputs: \n";
		
		    //For each hidden layer, print each connection for each neuron
		    for(int i = 0; i < HLayers.Count(); i++) {
			    print = print + "Hidden" + (i+1) + ": \n";
			    for(int j = 0; j < HLayers[i].Count(); j++) {
				    print = print + "\t" + HLayers[i][j].Name + ":\n";
				    for(int k = 0; k < HLayers[i][j].Dendrites.Count(); k++)
					    print = print + "\t\t(" + HLayers[i][j].Dendrites[k].Neuron.Name + ")" +
							    HLayers[i][j].Dendrites[k].Weight + "\n";
			    }
		    }
		
		    //print each connection for each neuron in the output layer
		    print = print + "Outputs: \n";
		    for(int i = 0; i < ONeurons.Count(); i++) {
			    print = print + "\t" + ONeurons[i].Name + ":\n";
			    for(int j = 0; j < ONeurons[i].Dendrites.Count(); j++)
				    print = print + "\t\t(" + ONeurons[i].Dendrites[j].Neuron.Name + ")" +
						    ONeurons[i].Dendrites[j].Weight + "\n";
		    }
		
		    return print;
	    }
        #endregion

        #region Setup/Packup
        public static void Save(Network network)
        {
            IFormatter formatter = new BinaryFormatter();
            var stream = new FileStream(network.Directory + network.Id + ".ann", FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, network);
            stream.Close();
        }


        public static Network Load(string filename)
        {
            IFormatter formatter = new BinaryFormatter();
            var stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
            var obj = (Network)formatter.Deserialize(stream);
            stream.Close();
            return obj;
        }

        public static string Serialize(Network network)
        {
            IFormatter formatter = new BinaryFormatter();
            var stream = new MemoryStream();
            formatter.Serialize(stream, network);
            
            stream.Position = 0;
            var str = "";
            using (var reader = new StreamReader(stream))
            {
                str = reader.ReadToEnd();
            }
            stream.Close();
            return str;
        }

        public static Network Deserialize(string str)
        {
            IFormatter formatter = new BinaryFormatter();
            byte[] byteArray = Encoding.ASCII.GetBytes(str);
            MemoryStream stream = new MemoryStream(byteArray);
            var network = (Network)formatter.Deserialize(stream);
            stream.Close();
            return network;
        }

        public static Network Copy(Network orig)
        {
            var network = new Network(orig.INeurons.Count, orig.HLayers.Select(l => l.Count).ToList(), orig.ONeurons.Count);
            var inputs = Input.Copy(orig.INeurons);
            var hiddenLayers = Dynamic.Copy(orig.HLayers);
            var outputs = Dynamic.Copy(orig.ONeurons);

            PlugIn(outputs, hiddenLayers, inputs);
            network.Id = orig.Id;
            network.Details = orig.Details;

            network.MaxEpochs = orig.MaxEpochs;
            network.MaxMinima = orig.MaxMinima;
            network.TargetError = orig.TargetError;
            network.Epochs = orig.Epochs;
            network.Error = orig.Error;

            network.INeurons = inputs;
            network.HLayers = hiddenLayers;
            network.ONeurons = outputs;

            return network;
        }

        public List<double> GetWeights()
        {
            var weights = new List<double>();
            foreach (var h in HLayers.SelectMany(l => l))
            {
                weights.AddRange(h.GetWeights());
            }
            foreach (var o in ONeurons)
            {
                weights.AddRange(o.GetWeights());
            }
            return weights;
        }

        public void SetWeights(List<double> weights)
        {
            var index = 0;
            foreach (var l in HLayers)
            {
                foreach (var h in l)
                {
                    h.SetWeights(weights.GetRange(index, h.Dendrites.Count()));
                    index += h.Dendrites.Count();
                }
            }
            foreach (var o in ONeurons)
            {
                o.SetWeights(weights.GetRange(index, o.Dendrites.Count()));
                index += o.Dendrites.Count();
            }
        }

        public static void PlugIn(List<Dynamic> outputs, List<List<Dynamic>> hiddens, List<Input> inputs)
        {
            foreach (var o in outputs)
            {
                o.PlugIn(hiddens[0]);
            }
            for (var l = 0; l < hiddens.Count - 1; l++)
            {
                foreach (var h in hiddens[l])
                {
                    h.PlugIn(hiddens[l + 1]);
                }
            }
            foreach (var h in hiddens[hiddens.Count - 1])
            {
                h.PlugIn(inputs);
            }
        }
        #endregion


        #region Analysis
        public List<NeuralPathway> GetPathways()
        {
            var pathways = new List<NeuralPathway>();
            foreach (var o in ONeurons)
            {
                GetPathways(o);
            }
            return pathways;
        }

        public List<NeuralPathway> GetPathways(Dynamic o)
        {
            var pathways = new List<NeuralPathway>();
            var path = new NeuralPathway();
            path.Path.Add(o);
            path.Weightings.Add(1);
            var paths = o.ExtendPathway(path);
            pathways.AddRange(paths);
            return pathways;
        }

        public Dictionary<string, double> RankInputs()
        {
            var rankings = new Dictionary<string, double>();
            foreach (var i in INeurons)
            {
                foreach (var p in GetPathways().Where(p => p.Path.Any(n => String.Equals(n.Name, i.Name, StringComparison.CurrentCultureIgnoreCase))))
                {
                    if (rankings.ContainsKey(i.Name))
                    {
                        if (rankings[i.Name] > p.WeightingProduct())
                        {
                            rankings[i.Name] = p.WeightingProduct();
                        }
                    }
                    else
                    {
                        rankings.Add(i.Name, p.WeightingProduct());
                    }

                }
            }
            return rankings;
        }
        #endregion
    }
}
