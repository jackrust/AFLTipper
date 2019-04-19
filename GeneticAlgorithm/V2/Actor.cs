using System;
using System.Collections.Generic;
using Utilities;

namespace GeneticAlgorithm.V2
{
    public class Actor
    {
        public string Id;
        public List<Gene> Genes { get; set; }
        public int Generation { get; set; }
        public double Error { get; set; }

        public static List<Gene> CreateGenes(List<GeneDefinition> geneDefinitions)
        {
            var s = BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0);
            var random = new Random(s);
            var genes = new List<Gene>();
            foreach (var definition in geneDefinitions)
            {
                var min = definition.Min;
                var max = definition.Max;
                genes.Add(new Gene()
                {
                    Max = definition.Max,
                    Min = definition.Min,
                    Value = (random.NextDouble() * (max - min)) + min
                });
            }
            return genes;
        }

        public static Actor Combine(Actor one, Actor two, int seed = 0)
        {
            var actor = new Actor();
            var genes = new List<Gene>();
            actor.Id = Guid.NewGuid().ToString();//"(" + one.Id + "&" + two.Id + ")";
            for(var i = 0; i < one.Genes.Count; i++)
            {
                var s = BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0);
                genes.Add(new Gene()
                {
                    Min = one.Genes[i].Min,
                    Max = one.Genes[i].Max,
                    Value =
                        Randomy.RandomNormalDistribution((one.Genes[i].Value + two.Genes[i].Value)/2, one.Genes[i].Min,
                            one.Genes[i].Max, s)
                });
            }
            actor.Genes = genes;
            return actor;
        }
    }
}
