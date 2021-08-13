using System;
using System.Collections.Generic;
using System.Linq;
using BookMarket.Data;
namespace Stas.Api.Data
{
    public static class DbInitializer
    {
        public static void Initialize(BookMarket_DBContext context)
        {
            context.Database.EnsureCreated();

            

        }
    }
}
