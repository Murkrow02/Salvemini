/*
See LICENSE folder for this sample’s licensing information.

Abstract:
Intent handler for OrderSoupIntents delivered by the system.
*/

using System;
using UIKit;
using TrainKit;
using SalveminiApp;
using System.IO;
using Xamarin.Essentials;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Json;
using Foundation;

namespace TrainKit
{
    public class TrainIntentHandler : TrainIntentHandling
    {
        RestApi.Models.Treno NextTrain = new RestApi.Models.Treno();

        override public void ConfirmTrain(TrainIntent intent, Action<TrainIntentResponse> completion)
        {
            // The confirm phase provides an opportunity for you to perform any 
            // final validation of the intent parameters and to verify that any 
            // needed services are available. You might confirm that you can 
            // communicate with your company’s server.

            completion(new TrainIntentResponse(TrainIntentResponseCode.Ready, null));
        }

        public override void HandleTrain(TrainIntent intent, Action<TrainIntentResponse> completion)
        {
            var filename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "OrariTreni.txt");
            
            
            var Trains = new List<RestApi.Models.Treno>();
            using (var client2 = new WebClient())
            {
                client2.DownloadFile("http://www.mysalvemini.me/Orari/OrariTreni.txt", filename);
            }
            var text = File.ReadAllText(filename);

            //GetTrains
            var defaults = new NSUserDefaults("group.com.codex.SalveminiApp");
            var stazione = defaults.IntForKey("savedStation");


            var value = JsonValue.Parse(text) as JsonArray;
            foreach (var obj in value)
            {
                Trains.Add(new RestApi.Models.Treno { Direzione = obj["Direzione"], Importanza = obj["Importanza"], Partenza = obj["Partenza"], Stazione = obj["Stazione"], Variazioni = obj["Variazioni"] });
            }

            ////Filter by station and direction
            Trains = Trains.Where(x => x.Direzione == true && x.Stazione == 0).OrderBy(x => x.LeaveTime).ToList();

            ////Remove Campania Express
            Trains = Trains.Where(x => x.Importanza != "EXP").ToList();
            bool done = false;
            //Check if there are more trains
            if (Trains[Trains.Count - 1].LeaveTime > new DateTime(1, 1, 1, DateTime.Now.Hour, DateTime.Now.Minute, 0))
            {
                //Remove previous trains
                Trains = Trains.Where(x => x.LeaveTime > new DateTime(1, 1, 1, DateTime.Now.Hour, DateTime.Now.Minute, 0)).ToList();
            }
            else
            {
                //Return first train of the day
                NextTrain = Trains[0];
                done = true;
            }

            ////Filter by Variations
            if (DateTime.Today.DayOfWeek == DayOfWeek.Sunday)
            {
                //Remove Feriali if sunday
                Trains = Trains.Where(x => x.Variazioni != "FER").ToList();
            }
            else
            {
                //Remove Festivi if not sunday
                Trains = Trains.Where(x => x.Variazioni != "FES").ToList();
            }

            //if (!done)
            NextTrain = Trains[0];
            completion(TrainIntentResponse.SuccessIntentResponseWithCitta(NextTrain.DirectionString, NextTrain.Partenza));
        }
    }
}
