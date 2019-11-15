using System;
using ObjCRuntime;

namespace SoupChef
{
	[Watch (5,0), NoTV, NoMac, iOS (12,0)]
	[Native]
	public enum OrarioIntentResponseCode : nint
	{
		Unspecified = 0,
		Ready,
		ContinueInApp,
		InProgress,
		Success,
		Failure,
		FailureRequiringAppLaunch
	}

	[Watch (5,0), NoTV, NoMac, iOS (12,0)]
	[Native]
	public enum TrenoIntentResponseCode : nint
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
