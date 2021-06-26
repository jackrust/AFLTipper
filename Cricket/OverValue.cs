namespace Cricket
{
    public class OverValue
    {
        public int Overs;
        public int Balls;

        public int TotalDeliveries()
        {
            return Overs * 6 + Balls;
        }
    }
}
