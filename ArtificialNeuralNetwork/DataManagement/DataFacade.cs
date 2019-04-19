using System.Collections.Generic;
using System.Linq;

namespace ArtificialNeuralNetwork.DataManagement
{
    public class DataFacade
    {
        public Data Data;
        protected Data DataCache;
        protected List<bool> Mask;
        public int Count;

        public List<bool> GetMask()
        {
            return Mask;
        }

        public virtual void SetMask(List<bool> mask)
        {
            Mask = mask;
            Count = mask.Count(m => m);
        }

        public virtual void SetData(Data data)
        {
            Data = data;
            DataCache = new Data {SuccessCondition = data.SuccessCondition};
            foreach (var dataPoint in data.DataPoints)
            {
                var subset = dataPoint.Inputs.Where((t, i) => Mask[i]).ToList();
                var dp = new DataPoint(subset, dataPoint.Outputs) {Reference = dataPoint.Reference};
                DataCache.DataPoints.Add(dp);
            }
        }

        public Data GetData()
        {
            return DataCache;
        }
    }
}
