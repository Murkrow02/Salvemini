using System;
using Foundation;
using Intents;
using ObjCRuntime;

namespace SalveminiApp
{
	// @interface TrainIntent : INIntent
	[Watch (5,0), NoTV, NoMac, iOS (12,0)]
	[BaseType (typeof(INIntent))]
	interface TrainIntent
	{
	}

	// @protocol TrainIntentHandling <NSObject>
	[Watch (5,0), NoTV, NoMac, iOS (12,0)]
	[Protocol, Model]
	[BaseType (typeof(NSObject))]
	 interface TrainIntentHandling
	{
		// @required -(void)handleTrain:(TrainIntent * _Nonnull)intent completion:(void (^ _Nonnull)(TrainIntentResponse * _Nonnull))completion;
		[Abstract]
		[Export ("handleTrain:completion:")]
		void HandleTrain (TrainIntent intent, Action<TrainIntentResponse> completion);

		// @optional -(void)confirmTrain:(TrainIntent * _Nonnull)intent completion:(void (^ _Nonnull)(TrainIntentResponse * _Nonnull))completion;
		[Export ("confirmTrain:completion:")]
		void ConfirmTrain (TrainIntent intent, Action<TrainIntentResponse> completion);
	}

	// @interface TrainIntentResponse : INIntentResponse
	[Watch (5,0), NoTV, NoMac, iOS (12,0)]
	[BaseType (typeof(INIntentResponse))]
	[DisableDefaultCtor]
	interface TrainIntentResponse
	{
		// -(instancetype _Nonnull)initWithCode:(TrainIntentResponseCode)code userActivity:(NSUserActivity * _Nullable)userActivity __attribute__((objc_designated_initializer));
		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		IntPtr Constructor (TrainIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		// +(instancetype _Nonnull)successIntentResponseWithCitta:(NSString * _Nonnull)citta ora:(NSString * _Nonnull)ora;
		[Static]
		[Export ("successIntentResponseWithCitta:ora:")]
		TrainIntentResponse SuccessIntentResponseWithCitta (string citta, string ora);

		// @property (readwrite, copy, nonatomic) NSString * _Nullable citta;
		[NullAllowed, Export ("citta")]
		string Citta { get; set; }

		// @property (readwrite, copy, nonatomic) NSString * _Nullable ora;
		[NullAllowed, Export ("ora")]
		string Ora { get; set; }

		// @property (readonly, nonatomic) TrainIntentResponseCode code;
		[Export ("code")]
		TrainIntentResponseCode Code { get; }
	}
}
