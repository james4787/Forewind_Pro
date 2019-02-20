using System.Runtime.InteropServices;
using DeadMosquito.GoogleMapsView.Internal;
using UnityEngine;

namespace DeadMosquito.GoogleMapsView
{
	using System;
	using JetBrains.Annotations;

	/// <summary>
	/// Class to display place picker activity
	/// </summary>
	[PublicAPI]
	public static class PlacePicker
	{
		static Action<Place> _onPlacePicked;
		static Action<string> _onFailure;

		/// <summary>
		/// Launch the place picker.
		/// 
		/// See more: https://developers.google.com/places/android-api/placepicker
		/// </summary>
		/// <param name="onPlacePicked">Invoked when user has successfully picked the place. Picked place is passed as a callback parameter.</param>
		/// <param name="onFailure">Invoked when picking place was cancelled or failed for some reason.</param>
		/// <exception cref="ArgumentNullException">When either one of callbacks is null.</exception>
		[PublicAPI]
		public static void ShowPlacePicker([NotNull] Action<Place> onPlacePicked, [NotNull] Action<string> onFailure)
		{
			if (GoogleMapUtils.IsPlatformNotSupported)
			{
				return;
			}

			if (onPlacePicked == null)
			{
				throw new ArgumentNullException("onPlacePicked");
			}

			if (onFailure == null)
			{
				throw new ArgumentNullException("onFailure");
			}

			_onPlacePicked = onPlacePicked;
			_onFailure = onFailure;

			GooglePlacesSceneHelper.Init();

			if (GoogleMapUtils.IsAndroid)
			{
				PlacePickerActivityUtils.LaunchPlacePicker();
			}

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS

			_googleMapsShowPlacePicker();

#endif
		}

		internal static void OnSuccess(string placeJson)
		{
			if (_onPlacePicked != null)
			{
				var place = Place.FromJson(placeJson);
				_onPlacePicked(place);
				_onPlacePicked = null;
			}
		}

		internal static void OnFailure(string message)
		{
			if (_onFailure != null)
			{
				_onFailure(message);
				_onFailure = null;
			}
		}

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
		[DllImport("__Internal")]
		static extern void _googleMapsShowPlacePicker();
#endif
	}
}