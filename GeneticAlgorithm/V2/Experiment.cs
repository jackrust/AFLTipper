using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneticAlgorithm.V2
{
    public class Experiment
    {
        public int Generations = 10;
        public int CorePopulation = 15;
        public int IdealPopulation
        {
            get { return (CorePopulation * (CorePopulation + 1)) / 2; }

        }
        public List<GeneDefinition> GeneDefinitions { get; set; }
        public List<Func<List<double>, double>> EvaluationCallbacks { get; set; }
        

        public Actor Best; 

        public void Run()
        {
            var actors = new List<Actor>();
            //Create population
            for (int i = 0; i < IdealPopulation; i++)
            {
                actors.Add(new Actor()
                {
                    Genes = Actor.CreateGenes(GeneDefinitions),
                    Generation = 0,
                    Id = i.ToString()
                });
            }
            //Loop over generations
            for(int i = 0; i < Generations; i++)
            {
                Console.WriteLine("Generation " + (i+1));

                //test
                for (int j = 0; j < actors.Count; j++)
                {
                    actors[j].Fitness = EvaluationCallbacks[i % EvaluationCallbacks.Count](actors[j].Genes.Select(g => g.Value).ToList());
                }
                //cull
                actors.Sort((x, y) => y.Fitness.CompareTo(x.Fitness));
                //Console.WriteLine(DetailActor(actors[0]));
                actors.RemoveRange(CorePopulation, actors.Count - CorePopulation);

                //repopulate
                for (int j = 0; j < CorePopulation && (actors.Count < IdealPopulation); j++)
                {
                    for (int k = 0; k < CorePopulation && (actors.Count < IdealPopulation); k++)
                    {
                        if (j != k)
                        {
                            var actor = Actor.Combine(actors[j], actors[k]);
                            actor.Generation = i + 1;
                            actors.Add(actor);
                        }
                    }
                }
            }

            Best = actors[0];
        }

        public double Test(Func<List<double>, double> callback)
        {
            return callback(Best.Genes.Select(g => g.Value).ToList());
        }

        public static string DetailActor(Actor actor)
        {
            var genes = actor.Genes.Select(i => String.Format("{0:N2}", i.Value)).Aggregate((i, j) => i.ToString() + "," + j.ToString());
            return String.Format("Generation:{0}, Error:{1:N2}, \nGenes:{2}\nId:{3}", actor.Generation, actor.Fitness, genes, actor.Id);
        }
    }
}
