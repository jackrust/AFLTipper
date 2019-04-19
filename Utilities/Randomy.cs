using System;

namespace Utilities
{
    public class Randomy
    {
        public static int Seed = DateTime.Now.Second; 

        public static double RandomNormalDistribution(double value, double min, double max, int seed = 0)
        {
            var random = new Random(seed == 0 ? Seed : seed);
            const int repetitions = 3;
            var total = 0.0;
            for (int i = 0; i < repetitions; i++)
            {
                total += random.NextDouble();
            }
            var average = total/repetitions;
            var positive = 0.5 < average;
            var delta = Math.Abs(average-0.5);
            var scaled = Numbery.Normalise(delta, 0, 1, min, max);
            var result = positive ? value + scaled : value - scaled;
            var clipped = result > max ? max : result < min ? min : result;
            return clipped;
        }
    }
}
