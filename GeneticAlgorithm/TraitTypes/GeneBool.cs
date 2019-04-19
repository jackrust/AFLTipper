using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm.TraitTypes
{
    public class GeneBool : Gene
    {
        public override double Encode()
        {
            Encoded = Decoded ? 1.0 : 0.0;
            return Encoded;
        }

        public override double Encode(dynamic decoded)
        {
            Decoded = decoded;
            return Encode();
        }

        public override dynamic Decode()
        {
            Decoded = Encoded > 0.5 ? true : false;
            return Decoded;
        }

        public override dynamic Decode(double encoded)
        {
            Encoded = encoded;
            return Decode();
        }
    }
}
