using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Essentials;

namespace SalveminiApp.Controls
{
    public partial class RadarView : ContentView
    {
        //size of the radar
        double radarRadius = 125;

        Location Center;

        double radius;

        public RadarView(double radius_, Location center_)
        {
            InitializeComponent();

            Center = center_;
            radius = radius_;

            
        }

        public void addPerson(Location person)
        {
            //Calculate distance between center and person in meters
            var distance = Location.CalculateDistance(Center, person, DistanceUnits.Kilometers) * 1000;

            //check person is not too far
            if (distance < radius)
            {
                //Calculate angular coefficient between start points
                var m = (person.Latitude - Center.Latitude) / (person.Longitude - Center.Longitude);

                //calculate x offset in metres
                var x = distance / Math.Sqrt(Math.Pow(m, 2) + 1);

                //Convert x in pixels
                var pixelX = (x * radarRadius) / radius;

                //Set negative value if longitude is less than the center
                if (person.Longitude < Center.Longitude)
                {
                    pixelX = -pixelX;
                }

                //calculate y offset in metres
                var y = m * x;

                //Convert y in pixels
                var pixelY = (y * radarRadius) / radius;

                //Set negative value if latitude is less than the center
                if (person.Latitude < Center.Latitude)
                {
                    pixelY = -pixelY;
                }

                //create person
                var child = new Frame { BackgroundColor = Color.FromHex("#f07aff"), HasShadow = false, Padding = 0, WidthRequest = 24, HeightRequest = 24, CornerRadius = 12 };
                AbsoluteLayout.SetLayoutFlags(child, AbsoluteLayoutFlags.PositionProportional);
                AbsoluteLayout.SetLayoutBounds(child, new Rectangle(.5, .5, 24, 24));

                //Translate
                child.TranslationX = pixelX;
                child.TranslationY = -pixelY;

                //Add person to layout
                radarlayout.Children.Add(child);
            }
        }
    }
}
