using System;
using System.Threading.Tasks;
using MonkeyCache.SQLite;
namespace SalveminiApp
{
    public class CacheHelper
    {

        public static Tipo GetCache<Tipo>(string chiave)
        {
            //Does this cache exists?
            if (string.IsNullOrEmpty(chiave) || !Barrel.Current.Exists(chiave))
                return default; //Nop, return null :(

            //Get object cache
            try
            {
                var cache = Barrel.Current.Get<object>(chiave);
                return (Tipo)cache; //Success deserializing object
            }
            catch
            {
                //Failed to deserialize cache, delete it and return null
                Barrel.Current.Empty(chiave);
                return default; 
            }
        }

    }
}
