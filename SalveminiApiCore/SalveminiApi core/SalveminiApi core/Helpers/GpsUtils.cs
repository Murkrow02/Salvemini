using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SalveminiApi_core
{
    public class Point
    {
        public decimal latitude { get; set; }
        public decimal longitude { get; set; }
    }

    public class UserPosition
    {
        public decimal xPosition { get; set; }
        public decimal yPosition { get; set; }
        public int Codice { get; set; }
    }

    public class GpsUtils
    {
        //Check if in circle GPS
        public static int IsInCircle(Point Activation, Point Person, decimal MeterRadius)
        {
            //Convert latitude in degrees
            decimal oneDegreeLatitude = 111320;

            //Get the radius in latitude
            decimal SquaredLatitudeRadius = (decimal)Math.Pow((double)MeterRadius / (double)oneDegreeLatitude, 2);

            //Get the distance
            decimal SquaredDistance = (decimal)Math.Pow((double)Person.latitude - (double)Activation.latitude, 2) + (decimal)Math.Pow((double)Person.longitude - (double)Activation.longitude, 2);

            if (SquaredDistance <= SquaredLatitudeRadius)
            {
                return 0; //Is in the circle
            }
            else
            {
                return Convert.ToInt32(Math.Sqrt((double)SquaredDistance) * (double)oneDegreeLatitude);
            }
        }
    }
}