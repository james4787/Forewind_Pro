using System;
using System.Runtime.InteropServices;
using DeadMosquito.GoogleMapsView.Internal;
using JetBrains.Annotations;

namespace DeadMosquito.GoogleMapsView
{
	public static class PlacesAPI
	{
		/// <summary>
		/// This method is for providing API key when running on iOS. MUST be called before using <see cref="PlacePicker"/>
		/// </summary>
		/// <param name="apiKey"></param>
		/// <exception cref="ArgumentNullException"></exception>
		[PublicAPI]
		public static void SetIosApiKey(string apiKey)
		{
			if (apiKey == null)
			{
				throw new ArgumentNullException("apiKey");
			}

			if (GoogleMapUtils.IsNotIosRuntime)
			{
				return;
			}

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
			_googlePlacesInit(apiKey);
#endif
		}

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
		[DllImport("__Internal")]
		static extern void _googlePlacesInit(string apiKey);
#endif
	}
}