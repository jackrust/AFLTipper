using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneticAlgorithm;
using Universal;
using Utilities;

namespace GeneticArtificialNeuralNetwork
{
    public class ANNOrganism : Organism
    {
        //Traits
        public int NumHiddenLayers;
        public List<int> NumInHidden;//2
        public int RangeSize;
        public int Epochs;
        public List<bool> Facade;//30

        //Trait Ranges
        public Range NumHiddenLayersRange = new Range(1,2);
        public int NumInHiddenCount = 2;
        public Range NumInHiddenRange = new Range(1, 5);
        public Range RangeSizeRange = new Range(1, 4);
        public Range EpochsRange = new Range(100, 1500);
        public int FacadeRangeCount = 30;
        public Range FacadeRange = new Range(0, 1);

        public void EncodeDNA()
        {
            DNA.Add(Numbery.Normalise(NumHiddenLayers, NumHiddenLayersRange.Lower,NumHiddenLayersRange.Upper, 0, 1 ));

            foreach (var num in NumInHidden)
            {
                DNA.Add(Numbery.Normalise(num, NumInHiddenRange.Lower, NumInHiddenRange.Upper, 0, 1));
            }

            DNA.Add(Numbery.Normalise(RangeSize, RangeSizeRange.Lower, RangeSizeRange.Upper, 0, 1));

            DNA.Add(Numbery.Normalise(Epochs, EpochsRange.Lower, EpochsRange.Upper, 0, 1));

            foreach (var datapoint in Facade)
            {
                DNA.Add(Numbery.Normalise(datapoint ? 1 : 0, FacadeRange.Lower, FacadeRange.Upper, 0, 1));
            }
            
        }

        public void DecodeDNA()
        {
            var currentDNACount = 0;
            NumHiddenLayers =
                (int)
                    Numbery.DenormaliseObsolete(DNA[currentDNACount], NumHiddenLayersRange.Lower,
                        NumHiddenLayersRange.Upper, 0, 1);
            currentDNACount++;

            NumInHidden = new List<int>();
            for (var i = 0; i < NumInHiddenCount; i++)//TODO: how do I know how many of these there are?
            {
                NumInHidden.Add(
                    (int)
                        Numbery.DenormaliseObsolete(DNA[currentDNACount], NumInHiddenRange.Lower, NumInHiddenRange.Upper, 0, 1));
                currentDNACount++;
            }

            RangeSize =
                (int)Numbery.DenormaliseObsolete(DNA[currentDNACount], RangeSizeRange.Lower, RangeSizeRange.Upper, 0, 1);
            currentDNACount++;

            Epochs = (int)Numbery.DenormaliseObsolete(DNA[currentDNACount], EpochsRange.Lower, EpochsRange.Upper, 0, 1);
            currentDNACount++;

            Facade = new List<bool>();
            for (var i = 0; i < FacadeRangeCount; i++)//TODO: how do I know how many of these there are?
            {
                Facade.Add(Numbery.DenormaliseObsolete(DNA[currentDNACount], FacadeRange.Lower, FacadeRange.Upper, 0, 1) > 0.5
                    ? true
                    : false);
                currentDNACount++;
            }
        }
    }
}
