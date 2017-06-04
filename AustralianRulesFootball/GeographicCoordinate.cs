namespace AustralianRulesFootball
{
    public class GeographicCoordinate
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public GeographicCoordinate()
        {
            Latitude = 0;
            Longitude = 0;
        }

        public GeographicCoordinate(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }
    }
}
