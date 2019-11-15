using System;
using Foundation;
using Intents;

namespace SoupChef
{
	// @interface OrarioIntent : INIntent
	[Watch (5,0), NoTV, NoMac, iOS (12,0)]
	[BaseType (typeof(INIntent))]
	interface OrarioIntent
	{
	}

	// @protocol OrarioIntentHandling <NSObject>
	[Watch (5,0), NoTV, NoMac, iOS (12,0)]
	[Protocol, Model]
	[BaseType (typeof(NSObject))]
	interface OrarioIntentHandling
	{
		// @required -(void)handleOrario:(OrarioIntent * _Nonnull)intent completion:(void (^ _Nonnull)(OrarioIntentResponse * _Nonnull))completion;
		[Abstract]
		[Export ("handleOrario:completion:")]
		void HandleOrario (OrarioIntent intent, Action<OrarioIntentResponse> completion);

		// @optional -(void)confirmOrario:(OrarioIntent * _Nonnull)intent completion:(void (^ _Nonnull)(OrarioIntentResponse * _Nonnull))completion;
		[Export ("confirmOrario:completion:")]
		void ConfirmOrario (OrarioIntent intent, Action<OrarioIntentResponse> completion);
	}

	// @interface OrarioIntentResponse : INIntentResponse
	[Watch (5,0), NoTV, NoMac, iOS (12,0)]
	[BaseType (typeof(INIntentResponse))]
	[DisableDefaultCtor]
	interface OrarioIntentResponse
	{
		// -(instancetype _Nonnull)initWithCode:(OrarioIntentResponseCode)code userActivity:(NSUserActivity * _Nullable)userActivity __attribute__((objc_designated_initializer));
		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		IntPtr Constructor (OrarioIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		// +(instancetype _Nonnull)successIntentResponseWithGiorno:(NSString * _Nonnull)giorno lezioni:(NSString * _Nonnull)lezioni;
		[Static]
		[Export ("successIntentResponseWithGiorno:lezioni:")]
		OrarioIntentResponse SuccessIntentResponseWithGiorno (string giorno, string lezioni);

		// +(instancetype _Nonnull)failureIntentResponseWithGiorno:(NSString * _Nonnull)giorno;
		[Static]
		[Export ("failureIntentResponseWithGiorno:")]
		OrarioIntentResponse FailureIntentResponseWithGiorno (string giorno);

		// @property (readwrite, copy, nonatomic) NSString * _Nullable giorno;
		[NullAllowed, Export ("giorno")]
		string Giorno { get; set; }

		// @property (readwrite, copy, nonatomic) NSString * _Nullable lezioni;
		[NullAllowed, Export ("lezioni")]
		string Lezioni { get; set; }

		// @property (readonly, nonatomic) OrarioIntentResponseCode code;
		[Export ("code")]
		OrarioIntentResponseCode Code { get; }
	}

	// @interface TrenoIntent : INIntent
	[Watch (5,0), NoTV, NoMac, iOS (12,0)]
	[BaseType (typeof(INIntent))]
	interface TrenoIntent
	{
	}

	// @protocol TrenoIntentHandling <NSObject>
	[Watch (5,0), NoTV, NoMac, iOS (12,0)]
	[Protocol, Model]
	[BaseType (typeof(NSObject))]
	interface TrenoIntentHandling
	{
		// @required -(void)handleTreno:(TrenoIntent * _Nonnull)intent completion:(void (^ _Nonnull)(TrenoIntentResponse * _Nonnull))completion;
		[Abstract]
		[Export ("handleTreno:completion:")]
		void HandleTreno (TrenoIntent intent, Action<TrenoIntentResponse> completion);

		// @optional -(void)confirmTreno:(TrenoIntent * _Nonnull)intent completion:(void (^ _Nonnull)(TrenoIntentResponse * _Nonnull))completion;
		[Export ("confirmTreno:completion:")]
		void ConfirmTreno (TrenoIntent intent, Action<TrenoIntentResponse> completion);
	}

	// @interface TrenoIntentResponse : INIntentResponse
	[Watch (5,0), NoTV, NoMac, iOS (12,0)]
	[BaseType (typeof(INIntentResponse))]
	[DisableDefaultCtor]
	interface TrenoIntentResponse
	{
		// -(instancetype _Nonnull)initWithCode:(TrenoIntentResponseCode)code userActivity:(NSUserActivity * _Nullable)userActivity __attribute__((objc_designated_initializer));
		[Export ("initWithCode:userActivity:")]
		[DesignatedInitializer]
		IntPtr Constructor (TrenoIntentResponseCode code, [NullAllowed] NSUserActivity userActivity);

		// +(instancetype _Nonnull)successIntentResponseWithCitta:(NSString * _Nonnull)citta ora:(NSString * _Nonnull)ora;
		[Static]
		[Export ("successIntentResponseWithCitta:ora:")]
		TrenoIntentResponse SuccessIntentResponseWithCitta (string citta, string ora);

		// @property (readwrite, copy, nonatomic) NSString * _Nullable citta;
		[NullAllowed, Export ("citta")]
		string Citta { get; set; }

		// @property (readwrite, copy, nonatomic) NSString * _Nullable ora;
		[NullAllowed, Export ("ora")]
		string Ora { get; set; }

		// @property (readonly, nonatomic) TrenoIntentResponseCode code;
		[Export ("code")]
		TrenoIntentResponseCode Code { get; }
	}
}
