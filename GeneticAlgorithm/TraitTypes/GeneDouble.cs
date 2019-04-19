using System;
using Utilities;

namespace GeneticAlgorithm.TraitTypes
{
    public class GeneDouble : Gene
    {
        public override double Encode()
        {
            //TODO: shouldn't really be using int min and max
            Encoded = Numbery.Normalise(Decoded, Int32.MinValue, Int32.MaxValue, 0, 1);
            return Encoded;
        }

        public override double Encode(dynamic decoded)
        {
            Decoded = decoded;
            return Encode();
        }

        public override dynamic Decode()
        {
            Decoded = (double)Numbery.DenormaliseObsolete(Encoded, Int32.MinValue, Int32.MaxValue, 0, 1);
            return Decoded;
        }

        public override dynamic Decode(double encoded)
        {
            Encoded = encoded;
            return Decode();
        }
    }
}
