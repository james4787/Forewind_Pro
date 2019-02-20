
using System.Runtime.InteropServices;
using DeadMosquito.GoogleMapsView.Internal;
using DeadMosquito.GoogleMapsView.MiniJSON;

namespace DeadMosquito.GoogleMapsView
{
	using System;
	using JniToolkit;
	using JetBrains.Annotations;

	/// <summary>
	/// Class to display place autocomplete activity
	/// </summary>
	[PublicAPI]
	public static class PlaceAutocomplete
	{
		static Action<Place> _onPlacePicked;
		static Action<string> _onFailure;

		/// <summary>
		/// Filter for customizing the autocomplete predictions.
		/// </summary>
		[PublicAPI]
		public enum AutocompleteFilter
		{
			/// <summary>
			/// An empty type filter.
			/// </summary>
			[PublicAPI]
			None = 0,

			/// <summary>
			/// Only return geocoding results, rather than business results.
			/// </summary>
			[PublicAPI]
			Geocode = 1007,

			/// <summary>
			/// Only return geocoding results with a precise address.
			/// </summary>
			[PublicAPI]
			Address = 2,

			/// <summary>
			/// Only return results that are classified as businesses.
			/// </summary>
			[PublicAPI]
			Establishment = 34,

			/// <summary>
			/// Return any result matching the following place types:
			///	* <see cref="Place.PlaceType.Locality"/>
			///	* <see cref="Place.PlaceType.Sublocality"/>
			///	* <see cref="Place.PlaceType.PostalCode"/>
			///	* <see cref="Place.PlaceType.Country"/>
			///	* <see cref="Place.PlaceType.AdministrativeAreaLevel1"/>
			///	* <see cref="Place.PlaceType.AdministrativeAreaLevel2"/>
			/// </summary>
			[PublicAPI]
			Regions = 4,
			
			/// <summary>
			/// Return any result matching the following place types:
			/// * <see cref="Place.PlaceType.AdministrativeAreaLevel3"/>
			/// * <see cref="Place.PlaceType.Locality"/>
			/// </summary>
			[PublicAPI]
			Cities = 5
		}

		/// <summary>
		/// Mode in which place autocompletion will be presented
		/// </summary>
		[PublicAPI]
		public enum Mode
		{
			/// <summary>
			/// Fullscreen mode
			/// </summary>
			[PublicAPI]
			Fullscreen = 1,

			/// <summary>
			/// Overlay mode
			/// </summary>
			[PublicAPI]
			Overlay = 2
		}

		/// <summary>
		/// Shows place autocomplete picker.
		/// See more: https://developers.google.com/places/android-api/autocomplete
		/// </summary>
		/// <param name="onPlacePicked">Invoked when user has successfully picked the place. Picked place is passed as a callback parameter.</param>
		/// <param name="onFailure">Invoked when picking place was cancelled or failed for some reason.</param>
		/// <param name="mode">Whether to show picker in fullscreen or overlay mode. Fullscreen is default.</param>
		/// <param name="filter">Filter results by place type. Default is no filtering.</param>
		/// <param name="countryCode">
		/// Filter autocomplete results to a specific country in ISO 3166-1 alpha-2 format. 
		/// See https://en.wikipedia.org/wiki/ISO_3166-1_alpha-2 for list of country codes.
		/// </param>
		/// <param name="boundsBias">Pass this to bias autocomplete results to a specific geographic region.</param>
		/// <exception cref="ArgumentNullException">When either one of callbacks is null.</exception>
		[PublicAPI]
		public static void ShowPlaceAutocomplete(
			[NotNull] Action<Place> onPlacePicked,
			[NotNull] Action<string> onFailure,
			[GoogleMapsAndroidOnly]Mode mode = Mode.Fullscreen, AutocompleteFilter filter = AutocompleteFilter.None,
			string countryCode = null, LatLngBounds boundsBias = null)
		{
			if (onPlacePicked == null)
			{
				throw new ArgumentNullException("onPlacePicked");
			}
			if (onFailure == null)
			{
				throw new ArgumentNullException("onFailure");
			}

			if (GoogleMapUtils.IsPlatformNotSupported)
			{
				return;
			}

			_onPlacePicked = onPlacePicked;
			_onFailure = onFailure;

			GooglePlacesSceneHelper.Init();

			if (GoogleMapUtils.IsAndroid)
			{
				PlacePickerActivityUtils.LaunchPlaceAutocomplete(mode, filter, countryCode, boundsBias);
			}
			
#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
			
			string bounds = null;
			if (boundsBias != null)
			{
				bounds = Json.Serialize(boundsBias.ToDictionary());
			}
			
			_googleMapsShowPlaceAutocomplete((int) mode, (int) filter, countryCode, bounds);
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
		static extern void _googleMapsShowPlaceAutocomplete(int mode, int filter, string countryCode, string bounds);
#endif
	}
}