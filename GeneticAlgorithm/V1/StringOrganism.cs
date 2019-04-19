using System;
using System.Collections.Generic;

namespace GeneticAlgorithm
{
    public class StringOrganism : Organism
    {
        public static int NumChromosones;
        public string Name;
        public List<StringGene> Genome;
        public List<Char> PotentialGenes;

        public StringOrganism()
        {
            NumChromosones = 0;
            Random = new Random();
            Genome = new List<StringGene>();
            PotentialGenes = new List<Char>();
        }

        public void ResetGenes()
        {
            Genome = new List<StringGene>();
            for (var i = 0; i < NumChromosones; i++)
            {
                var gene = new StringGene()
                {
                    Random = new Random(Random.Next())
                };
                gene.ResetGenes(PotentialGenes);
                Genome.Add(gene);
            }
        }

        public void SetGenes(List<StringGene> mothers, List<StringGene> fathers)
        {
            Genome = new List<StringGene>();
            for (int i = 0; i < mothers.Count && i < fathers.Count && i < NumChromosones; i++)
            {
                var geneSet = new StringGene()
                {
                    Random = mothers[i].Random
                };

                geneSet.Reproduce(mothers[i].Traits, fathers[i].Traits, PotentialGenes);
                Genome.Add(geneSet);
            }
        }
    }
}
