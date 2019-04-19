using System;
using System.Collections.Generic;

namespace GeneticAlgorithm
{
    public abstract class Actor
    {
        public string Name;
        public List<int> Generations = new List<int>();

        protected Actor()
        {
            Name = Guid.NewGuid().ToString();
        }

        public abstract double GetFitness();
    }
}
