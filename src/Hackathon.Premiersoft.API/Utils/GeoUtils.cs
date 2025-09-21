namespace Hackathon.Premiersoft.API.Utils
{
    public static class GeoUtils
    {
        public static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double raioTerra = 6371.0;

            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);

            var a = Math.Pow(Math.Sin(dLat / 2), 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Pow(Math.Sin(dLon / 2), 2);

            var c = 2 * Math.Asin(Math.Sqrt(a));

            return raioTerra * c;
        }

        private static double ToRadians(double angle) => Math.PI * angle / 180.0;
    }
}
