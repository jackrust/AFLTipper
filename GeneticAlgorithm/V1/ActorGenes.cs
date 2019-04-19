using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneticAlgorithm
{
    public abstract class ActorGenes
    {
        protected Random Random;

        protected ActorGenes()
        {
            Random = new Random();
        }

        protected static List<int> ResolveProbabilities(ActorGeneSetInt geneSet, List<int> populationAttributes)
        {
            var probabilities = new List<int>();
            for (var i = geneSet.Min; i < geneSet.Max; i++)
            {
                //Everyone should have at least a little chance
                probabilities.Add(i);

                var count = populationAttributes.Count(a => a == i);
                for (var j = 0; j < count; j++)
                {
                    //Weight towards the things that exist
                    for(var l = 0; l < populationAttributes.Count/100; l++)
                    {
                        probabilities.Add(i);
                    }
                    probabilities.Add(i);
                }
            }
            return probabilities;
        }

        //TODO: validate this
        protected static List<double> ResolveProbabilities(ActorGeneSetDouble geneSet, List<double> populationAttributes)
        {
            var probabilities = new List<double>();

            for (var i = Math.Floor(geneSet.Min); i < Math.Ceiling(geneSet.Max); i++)
            {
                //Everyone should have at least a little chance
                probabilities.Add(i);

                //we're effectively mapping back to ints here for better or worse
                var count = populationAttributes.Count(a => a > i -0.5 && a < i + 0.5);
                for (var j = 0; j < count; j++)
                {
                    //Weight towards the things that exist
                    for (var l = 0; l < populationAttributes.Count / 100; l++)
                    {
                        probabilities.Add(i);
                    }
                    probabilities.Add(i);
                }
            }
            return probabilities;
        }

        //TODO: validate this
        //TODO: can this just call the other version ^
        protected static List<List<double>> ResolveProbabilities(ActorGeneSetInt geneSetOuter, ActorGeneSetDouble geneSetInner, List<List<double>> populationAttributes)
        {
            var probabilityList = new List<List<double>>();
            for (var i = geneSetOuter.Min; i < geneSetOuter.Max; i++)
            {
                var probabilities = new List<double>();

                for (var j = Math.Floor(geneSetInner.Min); j < Math.Ceiling(geneSetInner.Max); j++)
                {
                    //Everyone should have at least a little chance
                    probabilities.Add(j);

                    if (populationAttributes.Count <= i) continue;

                    //we're effectively mapping back to ints here for better or worse
                    var count = populationAttributes[i].Count(a => a > j - 0.5 && a < j + 0.5);
                    for (var k = 0; k < count; k++)
                    {
                        //Weight towards the things that exist
                        for (var l = 0; l < populationAttributes.Count / 100; l++)
                        {
                            probabilities.Add(i);
                        }
                        probabilities.Add(j);
                    }
                }
                probabilityList.Add(probabilities);
            }
            return probabilityList;
        }

        protected int ResolveAttribute(List<int> list)
        {
            var i = Random.Next(0, list.Count);
            return list[i];
        }

        protected double ResolveAttribute(List<double> list)
        {
            var i = Random.Next(0, list.Count);
            return list[i] + (Random.NextDouble()-0.5);
        }
    }

    //TODO: replace gene properties with this
    public struct ActorGeneSetInt
    {
        public int Min;
        public int Max;
    }
    public struct ActorGeneSetDouble
    {
        public double Min;
        public double Max;
    }
}
