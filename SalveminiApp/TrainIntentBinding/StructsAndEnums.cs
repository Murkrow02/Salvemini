using System;
using ObjCRuntime;

namespace SalveminiApp
{
	//[Watch (5,0), NoTV, NoMac, iOS (12,0)]
	[Native]
	public enum TrainIntentResponseCode : long
	{
		Unspecified = 0,
		Ready,
		ContinueInApp,
		InProgress,
		Success,
		Failure,
		FailureRequiringAppLaunch
	}
}
