using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneticAlgorithm
{
    public class Organism
    {
        public Random Random;
        [Obsolete("Use Genes, then rename to DNA")]
        public List<double> DNA = new List<double>();
        public List<Gene> Genes = new List<Gene>();

        public void SetEncoded(List<double> encoded)
        {
            if (Genes.Count != encoded.Count)
                throw new GeneCountMismatchException();

            for (var i = 0; i < Genes.Count; i++)
            {
                Genes[i].Encoded = encoded[i];
            }
        }

        public List<double> GetEncoded()
        {
            return Genes.Select(x => x.Encoded).ToList();
        }

        public void Encode()
        {
            foreach (var gene in Genes)
            {
                gene.Encode();
            }
        }

        public void Decode()
        {
            foreach (var gene in Genes)
            {
                gene.Decode();
            }
        }
    }

    public class GeneCountMismatchException : Exception
    {
    }
}
