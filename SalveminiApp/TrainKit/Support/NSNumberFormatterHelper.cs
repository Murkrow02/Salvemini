using System;
using Foundation;
namespace TrainKit.Support
{
    public static class NSNumberFormatterHelper
    {
        public static NSNumberFormatter CurrencyFormatter
        {
            get
            {
                var formatter = new NSNumberFormatter();
                formatter.NumberStyle = NSNumberFormatterStyle.Currency;
                return formatter;
            }
        }
    }
}
