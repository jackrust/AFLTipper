using System;
using Utilities;

namespace GeneticAlgorithm.TraitTypes
{
    public class GeneListInt32 : Gene
    {
        public override double Encode()
        {
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
            Decoded = (int)Numbery.DenormaliseObsolete(Encoded, Int32.MinValue, Int32.MaxValue, 0, 1);
            return Decoded;
        }

        public override dynamic Decode(double encoded)
        {
            Encoded = encoded;
            return Decode();
        }
    }
}
