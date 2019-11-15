using System;
using ObjCRuntime;

namespace SalveminiApp
{
	[Watch(5, 0), NoTV, NoMac, iOS(12, 0)]
	[Native]
	public enum OrarioIntentResponseCode : long
	{
		Unspecified = 0,
		Ready,
		ContinueInApp,
		InProgress,
		Success,
		Failure,
		FailureRequiringAppLaunch
	}
	[Watch(5, 0), NoTV, NoMac, iOS(12, 0)]
	[Native]
	public enum TrenoIntentResponseCode : long
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
