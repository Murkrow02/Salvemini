using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace SalveminiApp.RestApi.Models
{
    public class Lezione
    {
        public int Giorno { get; set; }
        public int Ora { get; set; }
        public string Materia { get; set; }
        public string Sede { get; set; }
        public int idMateria { get; set; }

        //Internal Parameters
        public int numOre { get; set; }
        public bool toRemove { get; set; }
        public double OrarioFrameHeight { get; set; }
        public float OrarioFrameRadius { get; set; }

        public Thickness OrarioFrameMargin
        {
            get
            {
                var value = new Thickness(10, 2);

                return value;
            }
        }

        public string Colore
        {
            get
            {
                return Costants.SetColors(idMateria);
            }
        }

        public string oraLezione
        {
            get
            {
                return Costants.Ore[Ora];
            }
        }
    }

    public class newOrario
    {
        public int Giorno { get; set; }
        public int Ora { get; set; }
        public string Materia { get; }
        public string Sede { get; set; }
        public int idMateria { get; set; }
    }

   public static class extension
    {
        public static TValue GetValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue = default(TValue))
        {
            if (dictionary == null) { throw new ArgumentNullException(nameof(dictionary)); } // using C# 6
            if (key == null) { throw new ArgumentNullException(nameof(key)); } //  using C# 6

            TValue value;
            return dictionary.TryGetValue(key, out value) ? value : defaultValue;
        }
    }
}




