using System;
using System.Collections.Generic;
using System.Text;

namespace Sxb.Framework.Foundation
{
    public class GeoHelper
    {
        private const double EARTH_RADIUS = 6378137;//地球半径
        private static double Rad(double d)
        {
            return d * Math.PI / 180.0;
        }
        public static double GetDistance(double lat1, double lng1, double lat2, double lng2)
        {
            double radLat1 = Rad(lat1);
            double radLat2 = Rad(lat2);
            double a = radLat1 - radLat2;
            double b = Rad(lng1) - Rad(lng2);

            double s = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) + Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Pow(Math.Sin(b / 2), 2)));
            s = s * EARTH_RADIUS;
            s = Math.Round(s * 10000) / 10000;
            return s;
        }
        public static string FormatDistance(double meter)
        {
            meter = Math.Round(meter, 1);
            if (meter == 0)
            {
                return "";
            }
            else if (meter < 1000)
            {
                return meter.ToString() + "米";
            }
            else
            {
                return Math.Round(meter / 100) / 10 + "千米";
            }
        }
    }
}
