using DeadMosquito.GoogleMapsView;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class GooglePlacesDemo : MonoBehaviour
{
	[SerializeField] Text placeText;

	void Start()
	{
		// Sets the API key for iOS ONLY! For Android Provide your API key in AndroidManifest.xml
		SetApiKeyOnlyIOS();
	}
	
	static void SetApiKeyOnlyIOS()
	{
		const string apiKey = "YOUR_API_KEY_HERE";
		PlacesAPI.SetIosApiKey(apiKey);
	}
	
	[UsedImplicitly]
	public void OnShowPlacePicker()
	{
		PlacePicker.ShowPlacePicker(place =>
			{
				placeText.text = place.ToString();
			},
			error =>
			{
				var message = "Picking place failed, message: " + error;
				placeText.text = message;
				Debug.Log(message);
			});
	}

	[UsedImplicitly]
	public void OnShowAutocompletePickerSimple()
	{
		PlaceAutocomplete.ShowPlaceAutocomplete(place =>
			{
				placeText.text = place.ToString();
			},
			error =>
			{
				var message = "Picking place failed, message: " + error;
				placeText.text = message;
				Debug.Log(message);
			}, PlaceAutocomplete.Mode.Overlay);
	}
	
	[UsedImplicitly]
	public void OnShowPlaceAutocompleterPicker()
	{
		// Mode in which to show autocomplete: fullscreen or overlay
		const PlaceAutocomplete.Mode mode = PlaceAutocomplete.Mode.Fullscreen;

		// Filter places to cities only
		const PlaceAutocomplete.AutocompleteFilter filter = PlaceAutocomplete.AutocompleteFilter.Cities;

		// limit results to Australia
		const string countryCode = "AU";
		
		// limit search results in Sydney area
		var boundsBias = new LatLngBounds(
			new LatLng(-33.880490, 151.184363),
			new LatLng(-33.858754, 151.229596));
		
		PlaceAutocomplete.ShowPlaceAutocomplete(place =>
			{
				placeText.text = place.ToString();
			},
			error =>
			{
				var message = "Picking place failed, message: " + error;
				placeText.text = message;
				Debug.Log(message);
			}, mode, filter, countryCode, boundsBias);
	}
}