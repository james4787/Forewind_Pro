namespace DeadMosquito.GoogleMapsView.Internal
{
	using System;

	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Parameter, Inherited = false)]
	public class GoogleMapsAndroidOnlyAttribute : Attribute
	{
	}
}