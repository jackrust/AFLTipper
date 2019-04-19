using System.Collections.Generic;
using System.Linq;

namespace ArtificialNeuralNetwork.DataManagement
{
    public class DataFacadeGrouped : DataFacade
    {
        //No need for this to be passed in as yet. Constants compile better.
        private const int Grouping = 2;

        public override void SetMask(List<bool> mask)
        {
            Mask = mask;
            Count = mask.Count(m => m) * Grouping;
        }

        public override void SetData(Data data)
        {
            Data = data;
            DataCache = new Data { SuccessCondition = data.SuccessCondition };
            foreach (var dataPoint in data.DataPoints)
            {
                var subset = new List<double>();
                for(var i = 0; i < Mask.Count; i++)
                {
                    if (Mask[i])
                        subset.AddRange(dataPoint.Inputs.GetRange(i*Grouping, Grouping));
                }
                var dp = new DataPoint(subset, dataPoint.Outputs) { Reference = dataPoint.Reference };
                DataCache.DataPoints.Add(dp);
            }
        }
    }
}
