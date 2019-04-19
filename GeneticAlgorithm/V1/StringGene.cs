using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneticAlgorithm
{
    public class StringGene
    {
        private const int Variance = 2;
        public List<StringTrait> Traits;
        public Random Random;

        public StringGene()
        {
            Random = new Random();
        }

        public void ResetGenes(List<char> characters)
        {
            Traits = new List<StringTrait>();
            foreach (var character in characters)
            {
                var gene = new StringTrait()
                {
                    Random = new Random(Random.Next()),
                    Character = character,
                    Weighting = Random.Next(1, 100)
                };
                Traits.Add(gene);

            }
        }

        public void Reproduce(List<StringTrait> mothers, List<StringTrait> fathers, List<char> characters)
        {
            Traits = new List<StringTrait>();
            var paternal = false;
            foreach (var character in characters)
            {
                var mWeighting = mothers.First(g => g.Character == character).Weighting;
                var fWeighting = fathers.First(g => g.Character == character).Weighting;
                //var weighting = (mWeighting + fWeighting + Random.Next(-Variance, Variance) / 2);
                var weighting = (mWeighting + Random.Next(-Variance, Variance) / 2);
                if (paternal)
                {
                    weighting = (fWeighting + Random.Next(-Variance, Variance) / 2);
                }
                weighting = weighting > 100 ? 100 : weighting;
                weighting = weighting < 0 ? 0 : weighting;

                var gene = new StringTrait()
                {
                    Random = new Random(Random.Next()),
                    Character = character,
                    Weighting = weighting
                };
                Traits.Add(gene);

                paternal = !paternal;
            }
        }

        public StringTrait GetWeightedRandomGene()
        {
            var total = Traits.Sum(g => g.Weighting);
            var selected = Random.Next(0, total);

            StringTrait output = null;
            foreach (var gene in Traits)
            {
                if (selected < gene.Weighting)
                {
                    output = gene;
                    break;
                }

                selected = selected - gene.Weighting;
            }

            return output;
        }
    }
}
