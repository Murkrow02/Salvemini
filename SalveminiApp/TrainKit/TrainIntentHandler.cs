/*
See LICENSE folder for this sample’s licensing information.

Abstract:
Intent handler for OrderSoupIntents delivered by the system.
*/

using System;
using TrainKit;
using SalveminiApp;

namespace TrainKit
{
    public class TrainIntentHandler : TrainIntentHandling 
    {
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
            completion(TrainIntentResponse.SuccessIntentResponseWithCitta("Sorrento", "12 e 50"));
        }
    }
}
