using System;

namespace TrainKit.Support
{
    public interface ILocalizable
    {
        string LocalizedString { get; }
    }

    public interface ILocalizableCurrency
    {
        string LocalizedCurrencyValue { get; }
    }
}
