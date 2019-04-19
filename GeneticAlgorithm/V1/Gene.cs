namespace GeneticAlgorithm
{
    public abstract class Gene
    {
        public double Encoded { get; set; }
        public dynamic Decoded { get; set; }
        public string Reference { get; set; }

        public abstract double Encode();
        public abstract double Encode(dynamic decoded);

        public abstract dynamic Decode();
        public abstract dynamic Decode(double encoded);
    }
}
