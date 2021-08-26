using System;
using System.Collections.Generic;
using System.Linq;
using BookMarket;
using RandomDataGenerator.Randomizers;

namespace BookMarket.Data
{
    public static class DbInitializer
    {
        public static void Initialize(BookMarket_DBContext context)
        {
            return;
            context.Database.EnsureCreated();

            //Look if db is already seeded or running in release mode
            if (context.BookLibri.Any() || !Costants.IsDebug)
            {
                return;
            }


            //Add test users
            for (int i = 0; i < 200; i++)
            {
                context.BookUtenti.Add(new Models.BookUtenti { Nome = new RandomizerFirstName(new RandomDataGenerator.FieldOptions.FieldOptionsFirstName { Seed = i }).Generate(), Cognome = new RandomizerLastName(new RandomDataGenerator.FieldOptions.FieldOptionsLastName { Seed = i }).Generate(), Ip = "0.0.0.0", Mail = "Test", Password = "asd" , Token = "asdasd"});
            }

            context.SaveChanges();


            //Add test books
            var utenti = context.BookUtenti.ToList();
            for (int i = 0; i < 500; i++)
            {
                var Random = new Random();
                var RandomSubject = Random.Next(0, 12);
                context.BookLibri.Add(new Models.BookLibri { DataCaricamento = Utility.italianTime(), Codice = Utility.CreateToken(13), Materia = Costants.MarketSubjects()[RandomSubject], Nome = new RandomizerTextWords(new RandomDataGenerator.FieldOptions.FieldOptionsTextWords { Min = 1, Max = 4, UseNullValues = false, Seed = i }).Generate(), IdProprietario = utenti[Random.Next(0, 199)].Id, Prezzo = Random.Next(10, 500) });
            }

            context.SaveChanges();

        }

    }
}
