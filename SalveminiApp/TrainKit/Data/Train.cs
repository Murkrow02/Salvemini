/*
See LICENSE folder for this sample’s licensing information.

Abstract:
This type encapsulates the attributes of a soup order.
*/

using System;
using Foundation;
using TrainKit.Support;
using System.Linq;
using SalveminiApp;
using Intents;
using UIKit;

namespace TrainKit.Data
{
    public class Order : NSObject, ILocalizableCurrency, INSCoding
    {
        public NSDate Date { get; private set; }
        public NSUuid Identifier { get; private set; }
        public MenuItem MenuItem { get; private set; }
        public int Quantity { get; set; }
        public NSMutableSet<MenuItemOption> MenuItemOptions { get; set; }
        public NSDecimalNumber Total
        {
            get
            {
                return new NSDecimalNumber(Quantity).Multiply(MenuItem.Price);
            }
        }

        public Order(int quantity, MenuItem menuItem, NSMutableSet<MenuItemOption> menuItemOptions, NSDate date = null, NSUuid identifier = null)
        {
            Date = date ?? new NSDate();
            Identifier = identifier ?? new NSUuid();
            Quantity = quantity;
            MenuItem = menuItem;
            MenuItemOptions = menuItemOptions;
        }

        // SoupChef considers orders with the same contents (menuItem, quantity, 
        // menuItemOptions) to be identical. The data and idenfier properties 
        // are unique to an instance of an order (regardless of contents) and 
        // are not considered when determining equality.
        [Export("isEqual:")]
        override public bool IsEqual(NSObject anObject)
        {
            var other = anObject as Order;

            if (other is null)
            {
                return false;
            }

            if (ReferenceEquals(other, this))
            {
                return true;
            }

            // MenuItem check
            if (MenuItem is null)
            {
                if (!(other.MenuItem is null))
                {
                    return false;
                }
            }
            else
            {
                if (!MenuItem.IsEqual(other.MenuItem))
                {
                    return false;
                }
            }

            // Quantity check
            if (Quantity != other.Quantity)
            {
                return false;
            }

            // MenuItemOptions check
            if (MenuItemOptions is null)
            {
                if (!(other.MenuItemOptions is null))
                {
                    return false;
                }
            }
            else
            {
                if (!MenuItemOptions.IsEqualToSet(other.MenuItemOptions))
                {
                    return false;
                }
            }

            return true;
        }

        public override nuint GetNativeHash()
        {
            nuint hashValue = (MenuItem is null) ? 0 : MenuItem.GetNativeHash();
            hashValue = hashValue ^ (nuint)Quantity.GetHashCode();
            hashValue = hashValue ^ (MenuItemOptions is null ? 0 : MenuItemOptions.GetNativeHash());
            return hashValue;
        }

        #region LocalizableCurrency (and other localization methods)
        public string LocalizedCurrencyValue
        {
            get
            {
                return NSNumberFormatterHelper.CurrencyFormatter.StringFromNumber(Total) ?? "";
            }
        }

        public string[] LocalizedOptionsArray
        {
            get
            {
                var localizedArray = MenuItemOptions
                    .ToArray<MenuItemOption>()
                    .Select(arg => arg.LocalizedString)
                    .OrderBy(arg => arg, StringComparer.CurrentCultureIgnoreCase)
                    .ToArray<string>();
                return localizedArray;
            }
        }

        public string LocalizedOptionString
        {
            get
            {
                return String.Join(", ", LocalizedOptionsArray);
            }
        }
        #endregion

        #region intent helpers
        public TrainIntent Intent
        {
            get
            {
                var orderSoupIntent = new TrainIntent();
                
              
              
                var comment = "Suggested phrase for ordering a specific soup";
                var phrase = NSBundleHelper.TrainKitBundle.GetLocalizedString("ANSWER", comment);
                orderSoupIntent.SuggestedInvocationPhrase = String.Format(phrase, MenuItem.LocalizedString);

                return orderSoupIntent;
            }
        }

        public static Order FromOrderSoupIntent(TrainIntent intent)
        {
            var menuManager = new SoupMenuManager();

            var soupID = intent.Identifier.ToString();
            if (soupID is null)
            {
                return null;
            }

            var menuItem = menuManager.FindItem(soupID);
            if (menuItem is null)
            {
                return null;
            }

            var quantity = 2;
            if (menuItem is null)
            {
                return null;
            }

            MenuItemOption[] rawOptions;
            
                rawOptions = new MenuItemOption[0];
           

            var order = new Order(quantity, menuItem, new NSMutableSet<MenuItemOption>(rawOptions));

            return order;
        }
        #endregion

        #region INSCoding
        [Export("initWithCoder:")]
        public Order(NSCoder coder)
        {
            Date = (NSDate)coder.DecodeObject("Date");
            Identifier = (NSUuid)coder.DecodeObject("Identifier");
            MenuItem = (MenuItem)coder.DecodeObject("MenuItem");
            Quantity = (int)coder.DecodeInt("Quantity");

            // Can't decode an NSMutableSet<MenuItemOption> directly. Get an
            // NSSet, convert it to NSMutableSet<MenuItemOption>
            var set = (NSSet)(coder.DecodeObject("MenuItemOptions"));
            MenuItemOptions = new NSMutableSet<MenuItemOption>(set.ToArray<MenuItemOption>());
        }

        public void EncodeTo(NSCoder encoder)
        {
            encoder.Encode(Date, "Date");
            encoder.Encode(Identifier, "Identifier");
            encoder.Encode(MenuItem, "MenuItem");
            encoder.Encode(Quantity, "Quantity");
            encoder.Encode(MenuItemOptions, "MenuItemOptions");
        }
        #endregion
    }
}
