namespace Tipper.Betting
{
    public class BettingRule
    {
        public int Priority;
        public double Wager;
        public double Threshold;

        public double Scenario(double margin)
        {
            return margin > Threshold ? Wager : 0;
        }
    }
}
