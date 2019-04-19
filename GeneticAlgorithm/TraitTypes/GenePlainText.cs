using System;
using System.Collections.Generic;
using System.Globalization;
using Utilities;

namespace GeneticAlgorithm.TraitTypes
{
    public class GenePlainText : Gene
    {
        public static List<char> AllowedCharacters = new List<char>{' ', 
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'Q', 'Y', 'Z', 
            '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '.', '!', '?'};
        public int Min = 0;
        public int Max = AllowedCharacters.Count;

        public override double Encode()
        {
            var index = AllowedCharacters.IndexOf(Decoded);
            Encoded = Numbery.Normalise(index, Min, Max, 0, 1);
            return Encoded;
        }

        public override double Encode(dynamic decoded)
        {
            Decoded = decoded;
            return Encode();
        }

        public override dynamic Decode()
        {
            var index = (int)Numbery.DenormaliseObsolete(Encoded, Min, Max, 0, 1);
            Decoded = AllowedCharacters[index];
            return Decoded;
        }

        public override dynamic Decode(double encoded)
        {
            Encoded = encoded;
            return Decode();
        }
    }
}
