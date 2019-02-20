
namespace DeadMosquito.GoogleMapsView
{
	using System;
	using JniToolkit;
	using JetBrains.Annotations;
	using UnityEngine;

	[PublicAPI]
	public static class PlacePickerActivityUtils
	{
		const string PickerActivityClass = "com.deadmosquitogames.gmaps.places.PickerActivity";
		
		const string EXTRAS_PICKER_TYPE = "EXTRAS_PICKER_TYPE";
		
		// Autocomplete
		const string EXTRAS_MODE = "EXTRAS_MODE";
		const string EXTRAS_FILTER = "EXTRAS_FILTER";
		const string EXTRAS_COUNTRY_CODE = "EXTRAS_COUNTRY_CODE";
		const string EXTRAS_BOUNDS_BIAS = "EXTRAS_BOUNDS_BIAS";

		
		const int PLACE_PICKER_REQUEST = 111;
		const int AUTOCOMPLETE_PICKER_REQUEST = 222;
		
		const int FLAG_NO_ANIMATION = 65536;
		const string PutExtraMethod = "putExtra";

		[PublicAPI]
		public static void LaunchPlacePicker()
		{
			Launch(PLACE_PICKER_REQUEST);
		}

		[PublicAPI]
		public static void LaunchPlaceAutocomplete(PlaceAutocomplete.Mode mode, PlaceAutocomplete.AutocompleteFilter filter, string countryCode, LatLngBounds boundsBias)
		{
			Launch(AUTOCOMPLETE_PICKER_REQUEST, intent =>
			{
				intent.CallAJO(PutExtraMethod, EXTRAS_MODE, (int) mode);
				intent.CallAJO(PutExtraMethod, EXTRAS_FILTER, (int) filter);
				if (countryCode != null)
				{
					intent.CallAJO(PutExtraMethod, EXTRAS_COUNTRY_CODE, countryCode);
				}
				if (boundsBias != null)
				{
					intent.CallAJO(PutExtraMethod, EXTRAS_BOUNDS_BIAS, boundsBias.ToAJO());
				}
			});
		}

		static void Launch(int request, Action<AndroidJavaObject> intentAction = null)
		{
			InitGoogleApi();

			var intent = new AndroidJavaObject("android.content.Intent", JniToolkitUtils.Activity, JniToolkitUtils.ClassForName(PickerActivityClass));
			intent.CallAJO(PutExtraMethod, EXTRAS_PICKER_TYPE, request);
			intent.CallAJO("addFlags", FLAG_NO_ANIMATION);
			if (intentAction != null)
			{
				intentAction(intent);
			}
			JniToolkitUtils.StartActivity(intent);
		}

		static void InitGoogleApi()
		{
			PickerActivityClass.AJCCallStaticOnce("init", JniToolkitUtils.Activity);
		}
		
	}
}