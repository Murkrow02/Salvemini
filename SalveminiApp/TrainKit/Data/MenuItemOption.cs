using System;
using System.Linq;
using Foundation;
using TrainKit.Support;

namespace TrainKit.Data
{
    public class MenuItemOption : NSObject, ILocalizable, INSCoding
    {
        public const string Cheese = "TRAIN";
        public const string RedPepper = "HOUR";
        public const string Croutons = "CITY";

        public static string[] All = new string[] { Cheese, RedPepper, Croutons };

        public string LocalizedString
        {
            get
            {
                string usageComment = $"UI representation for MenuItemOption value: {Value}";
                return NSBundleHelper.TrainKitBundle.GetLocalizedString(Value, usageComment, null);
            }
        }

        public MenuItemOption(string value)
        {
            if (All.Contains(value))
            {
                _value = value;
            }
            else
            {
                throw new ArgumentException($"Invalid menuItemOption value: {value}");
            }
        }

        string _value;
        public string Value
        {
            get { return _value; }
        }

        [Export("isEqual:")]
        override public bool IsEqual(NSObject anObject)
        {
            var other = anObject as MenuItemOption;
            if (other is null)
            {
                return false;
            }
            if (ReferenceEquals(other, this))
            {
                return true;
            }
            return Value.Equals(other.Value);
        }

        public override nuint GetNativeHash()
        {
            return !(Value is null) ? (nuint)this.Value?.GetHashCode() : 0;
        }

        #region INSCoding
        [Export("initWithCoder:")]
        public MenuItemOption(NSCoder coder)
        {
            _value = (NSString)coder.DecodeObject("Value");
        }

        public void EncodeTo(NSCoder encoder)
        {
            encoder.Encode((NSString)_value, "Value");
        }
        #endregion
    }
}
