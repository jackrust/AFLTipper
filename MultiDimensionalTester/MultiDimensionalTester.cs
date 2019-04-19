using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace MultiDimensionalTester
{
    public class MultiDimensionalTester
    {
        public Func<object[], string> Callback;
        public List<ParameterRange> ParameterRanges = new List<ParameterRange>();

        //TODO: these should be parameters
        //public object Tipper;

        public string Run(int skip = 0, int until = 0)
        {
            var result = "";
            if (Callback == null || !ParameterRanges.Any()) return result;

            var numScenarios = ParameterRanges.Select(pr => pr.Options.Count()).Aggregate((p, c) => p * c);
            var parameterIndexes = new List<int>(ParameterRanges.Select(pr => 0));
            var scenarios = new List<object[]>();

            //prepare scenario parameters
            for (var i = 0; i < numScenarios; i++)
            {
                //Create the scenario parameters object
                scenarios.Add(new object[ParameterRanges.Count()]);
                //Add parameters
                for (var j = 0; j < parameterIndexes.Count; j++)
                {
                    scenarios[i][j] = ParameterRanges[j].Options[parameterIndexes[j]];
                }

                //increase indexes
                IncrementIndexes(parameterIndexes, ParameterRanges.Select(pr => pr.Options.Count()).ToList());

            }

            //run scenario
            //TODO: pull into loop with scenario generation?
            for (var i = skip; i < numScenarios && (until == 0 || i < until); i++)
            {
                result = result + "\n" +  Callback(scenarios[i]);
            }

            return result;
        }

        public void IncrementIndexes(List<int> indexes, List<int> counts)
        {
            if (indexes.Count != counts.Count)
                throw new InvalidOperationException("IncrementIndexes: indexes and counts must match");
            var i = 0;
            while (true)
            {
                if (i >= indexes.Count)
                    break;

                indexes[i]++;
                if (indexes[i] >= counts[i])
                {
                    indexes[i] = 0;
                    i++;
                    continue;
                }
                break;
            }
        }

        //public void AddTipper(object tipper)
        //{
        //    Tipper = tipper;
        //}

        public void AddParameterGroup(List<string> parameters)
        {
            var pr = new ParameterRange();
            pr.Options = parameters.Select(p => p as object).ToArray();
            ParameterRanges.Add(pr);
        }

        public void AddParameterGroup(List<int> parameters)
        {
            var pr = new ParameterRange();
            pr.Options = parameters.Select(p => p as object).ToArray();
            ParameterRanges.Add(pr);
        }
    }

    public class ParameterRange
    {
        public string Name;
        public object[] Options;
    }
}
