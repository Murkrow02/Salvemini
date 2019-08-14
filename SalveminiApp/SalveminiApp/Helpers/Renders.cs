using System;
using Xamarin.Forms;

namespace SalveminiApp
{
    public class ShadowFrame : Frame
    {
        
    }

    public class TransparentGradient : Frame
    {

    }

    public class GradientFrame : Frame
    {
        public Color StartColor
        {
            get { return (Color)GetValue(StartColorProperty); }
            set { SetValue(StartColorProperty, value); }
        }
        public Color EndColor
        {
            get;
            set;
        }

        private static BindableProperty StartColorProperty = BindableProperty.Create(
                                                         propertyName: "StartColor",
                                                         returnType: typeof(Color),
                                                         declaringType: typeof(GradientFrame),
                                                         defaultValue: "#000000",
                                                         defaultBindingMode: BindingMode.TwoWay,
                                                         propertyChanged: StartColorPropertyChanged);


        private static void StartColorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (GradientFrame)bindable;
            control.StartColor = (Color)newValue;
        }

    }
}
