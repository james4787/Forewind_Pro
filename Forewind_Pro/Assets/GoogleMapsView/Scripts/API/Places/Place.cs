using System;
using System.Collections.Generic;
using DeadMosquito.GoogleMapsView.Internal;
using DeadMosquito.GoogleMapsView.MiniJSON;
using GoogleMapsView.Scripts.Internal.Utils;
using JetBrains.Annotations;
using UnityEngine;

namespace DeadMosquito.GoogleMapsView
{
	/// <summary>
	///     Represents a particular physical place.
	///     A Place encapsulates information about a physical location, including its name, address, and any other information
	///     we might have about it.
	///     Note that generally some fields will be inapplicable to certain places, or the information may be unknown.
	/// </summary>
	[PublicAPI]
	public sealed class Place
	{
		static Dictionary<string, PlaceType> _iosPlaceMappings = new Dictionary<string, PlaceType>
		{
			{"accounting", PlaceType.Accounting},
			{"airport", PlaceType.Airport},
			{"amusement_park", PlaceType.AmusementPark},
			{"aquarium", PlaceType.Aquarium},
			{"art_gallery", PlaceType.ArtGallery},
			{"atm", PlaceType.Atm},
			{"bakery", PlaceType.Bakery},
			{"bank", PlaceType.Bank},
			{"bar", PlaceType.Bar},
			{"beauty_salon", PlaceType.BeautySalon},
			{"bicycle_store", PlaceType.BicycleStore},
			{"book_store", PlaceType.BookStore},
			{"bowling_alley", PlaceType.BowlingAlley},
			{"bus_station", PlaceType.BusStation},
			{"cafe", PlaceType.Cafe},
			{"campground", PlaceType.Campground},
			{"car_dealer", PlaceType.CarDealer},
			{"car_rental", PlaceType.CarRental},
			{"car_repair", PlaceType.CarRepair},
			{"car_wash", PlaceType.CarWash},
			{"casino", PlaceType.Casino},
			{"cemetery", PlaceType.Cemetery},
			{"church", PlaceType.Church},
			{"city_hall", PlaceType.CityHall},
			{"clothing_store", PlaceType.ClothingStore},
			{"convenienceStore", PlaceType.ConvenienceStore},
			{"courthouse", PlaceType.Courthouse},
			{"dentist", PlaceType.Dentist},
			{"department_store", PlaceType.DepartmentStore},
			{"doctor", PlaceType.Doctor},
			{"electrician", PlaceType.Electrician},
			{"electronics_store", PlaceType.ElectronicsStore},
			{"embassy", PlaceType.Embassy},
			{"fire_station", PlaceType.FireStation},
			{"florist", PlaceType.Florist},
			{"funeral_home", PlaceType.FuneralHome},
			{"furniture_store", PlaceType.FurnitureStore},
			{"gas_station", PlaceType.GasStation},
			{"gym", PlaceType.Gym},
			{"hair_care", PlaceType.HairCare},
			{"hardware_store", PlaceType.HardwareStore},
			{"hindu_temple", PlaceType.HinduTemple},
			{"home_goods_store", PlaceType.HomeGoodsStore},
			{"hospital", PlaceType.Hospital},
			{"insurance_agency", PlaceType.InsuranceAgency},
			{"jewelry_store", PlaceType.JewelryStore},
			{"laundry", PlaceType.Laundry},
			{"lawyer", PlaceType.Lawyer},
			{"library", PlaceType.Library},
			{"liquor_store", PlaceType.LiquorStore},
			{"local_government_office", PlaceType.LocalGovernmentOffice},
			{"locksmith", PlaceType.Locksmith},
			{"lodging", PlaceType.Lodging},
			{"meal_delivery", PlaceType.MealDelivery},
			{"meal_takeaway", PlaceType.MealTakeaway},
			{"mosque", PlaceType.Mosque},
			{"movie_rental", PlaceType.MovieRental},
			{"movie_theater", PlaceType.MovieTheater},
			{"moving_company", PlaceType.MovingCompany},
			{"museum", PlaceType.Museum},
			{"night_club", PlaceType.NightClub},
			{"painter", PlaceType.Painter},
			{"park", PlaceType.Park},
			{"parking", PlaceType.Parking},
			{"pet_store", PlaceType.PetStore},
			{"pharmacy", PlaceType.Pharmacy},
			{"physiotherapist", PlaceType.Physiotherapist},
			{"plumber", PlaceType.Plumber},
			{"police", PlaceType.Police},
			{"post_office", PlaceType.PostOffice},
			{"real_estate_agency", PlaceType.RealEstateAgency},
			{"restaurant", PlaceType.Restaurant},
			{"roofing_contractor", PlaceType.RoofingContractor},
			{"rv_park", PlaceType.RvPark},
			{"school", PlaceType.School},
			{"shoe_store", PlaceType.ShoeStore},
			{"shopping_mall", PlaceType.ShoppingMall},
			{"spa", PlaceType.Spa},
			{"stadium", PlaceType.Stadium},
			{"storage", PlaceType.Storage},
			{"store", PlaceType.Store},
			{"subway_station", PlaceType.SubwayStation},
			{"supermarket", PlaceType.GroceryOrSupermarket},
			{"synagogue", PlaceType.Synagogue},
			{"taxi_stand", PlaceType.TaxiStand},
			{"train_station", PlaceType.TrainStation},
			{"transit_station", PlaceType.TransitStation},
			{"travel_agency", PlaceType.TravelAgency},
			{"veterinary_care", PlaceType.VeterinaryCare},
			{"zoo", PlaceType.Zoo},
			{"administrative_area_level_1", PlaceType.AdministrativeAreaLevel1},
			{"administrative_area_level_2", PlaceType.AdministrativeAreaLevel2},
			{"administrative_area_level_3", PlaceType.AdministrativeAreaLevel3},
			{"administrative_area_level_4", PlaceType.AdministrativeAreaLevel4},
			{"administrative_area_level_5", PlaceType.AdministrativeAreaLevel5},
			{"colloquial_area", PlaceType.ColloquialArea},
			{"country", PlaceType.Country},
			{"establishment", PlaceType.Establishment},
			{"finance", PlaceType.Finance},
			{"floor", PlaceType.Floor},
			{"food", PlaceType.Food},
			{"general_contractor", PlaceType.GeneralContractor},
			{"geocode", PlaceType.Geocode},
			{"health", PlaceType.Health},
			{"intersection", PlaceType.Intersection},
			{"locality", PlaceType.Locality},
			{"natural_feature", PlaceType.NaturalFeature},
			{"neighborhood", PlaceType.Neighborhood},
			{"place_of_worship", PlaceType.PlaceOfWorship},
			{"political", PlaceType.Political},
			{"point_of_interest", PlaceType.PointOfInterest},
			{"post_box", PlaceType.PostBox},
			{"postal_code", PlaceType.PostalCode},
			{"postal_code_prefix", PlaceType.PostalCodePrefix},
			{"postal_code_suffix", PlaceType.PostalCode},
			{"postal_town", PlaceType.PostalTown},
			{"premise", PlaceType.Premise},
			{"room", PlaceType.Room},
			{"route", PlaceType.Route},
			{"street_address", PlaceType.StreetAddress},
			{"street_number", PlaceType.StreetAddress},
			{"sublocality", PlaceType.Sublocality},
			{"sublocality_level_4", PlaceType.SublocalityLevel4},
			{"sublocality_level_5", PlaceType.SublocalityLevel5},
			{"sublocality_level_3", PlaceType.SublocalityLevel3},
			{"sublocality_level_2", PlaceType.SublocalityLevel2},
			{"sublocality_level_1", PlaceType.SublocalityLevel1},
			{"subpremise", PlaceType.Subpremise}
		};

		public enum PlaceType
		{
			[PublicAPI] Other = 0,
			[PublicAPI] Accounting = 1,
			[PublicAPI] Airport = 2,
			[PublicAPI] AmusementPark = 3,
			[PublicAPI] Aquarium = 4,
			[PublicAPI] ArtGallery = 5,
			[PublicAPI] Atm = 6,
			[PublicAPI] Bakery = 7,
			[PublicAPI] Bank = 8,
			[PublicAPI] Bar = 9,
			[PublicAPI] BeautySalon = 10,
			[PublicAPI] BicycleStore = 11,
			[PublicAPI] BookStore = 12,
			[PublicAPI] BowlingAlley = 13,
			[PublicAPI] BusStation = 14,
			[PublicAPI] Cafe = 15,
			[PublicAPI] Campground = 16,
			[PublicAPI] CarDealer = 17,
			[PublicAPI] CarRental = 18,
			[PublicAPI] CarRepair = 19,
			[PublicAPI] CarWash = 20,
			[PublicAPI] Casino = 21,
			[PublicAPI] Cemetery = 22,
			[PublicAPI] Church = 23,
			[PublicAPI] CityHall = 24,
			[PublicAPI] ClothingStore = 25,
			[PublicAPI] ConvenienceStore = 26,
			[PublicAPI] Courthouse = 27,
			[PublicAPI] Dentist = 28,
			[PublicAPI] DepartmentStore = 29,
			[PublicAPI] Doctor = 30,
			[PublicAPI] Electrician = 31,
			[PublicAPI] ElectronicsStore = 32,
			[PublicAPI] Embassy = 33,
			[PublicAPI] Establishment = 34,
			[PublicAPI] Finance = 35,
			[PublicAPI] FireStation = 36,
			[PublicAPI] Florist = 37,
			[PublicAPI] Food = 38,
			[PublicAPI] FuneralHome = 39,
			[PublicAPI] FurnitureStore = 40,
			[PublicAPI] GasStation = 41,
			[PublicAPI] GeneralContractor = 42,
			[PublicAPI] GroceryOrSupermarket = 43,
			[PublicAPI] Gym = 44,
			[PublicAPI] HairCare = 45,
			[PublicAPI] HardwareStore = 46,
			[PublicAPI] Health = 47,
			[PublicAPI] HinduTemple = 48,
			[PublicAPI] HomeGoodsStore = 49,
			[PublicAPI] Hospital = 50,
			[PublicAPI] InsuranceAgency = 51,
			[PublicAPI] JewelryStore = 52,
			[PublicAPI] Laundry = 53,
			[PublicAPI] Lawyer = 54,
			[PublicAPI] Library = 55,
			[PublicAPI] LiquorStore = 56,
			[PublicAPI] LocalGovernmentOffice = 57,
			[PublicAPI] Locksmith = 58,
			[PublicAPI] Lodging = 59,
			[PublicAPI] MealDelivery = 60,
			[PublicAPI] MealTakeaway = 61,
			[PublicAPI] Mosque = 62,
			[PublicAPI] MovieRental = 63,
			[PublicAPI] MovieTheater = 64,
			[PublicAPI] MovingCompany = 65,
			[PublicAPI] Museum = 66,
			[PublicAPI] NightClub = 67,
			[PublicAPI] Painter = 68,
			[PublicAPI] Park = 69,
			[PublicAPI] Parking = 70,
			[PublicAPI] PetStore = 71,
			[PublicAPI] Pharmacy = 72,
			[PublicAPI] Physiotherapist = 73,
			[PublicAPI] PlaceOfWorship = 74,
			[PublicAPI] Plumber = 75,
			[PublicAPI] Police = 76,
			[PublicAPI] PostOffice = 77,
			[PublicAPI] RealEstateAgency = 78,
			[PublicAPI] Restaurant = 79,
			[PublicAPI] RoofingContractor = 80,
			[PublicAPI] RvPark = 81,
			[PublicAPI] School = 82,
			[PublicAPI] ShoeStore = 83,
			[PublicAPI] ShoppingMall = 84,
			[PublicAPI] Spa = 85,
			[PublicAPI] Stadium = 86,
			[PublicAPI] Storage = 87,
			[PublicAPI] Store = 88,
			[PublicAPI] SubwayStation = 89,
			[PublicAPI] Synagogue = 90,
			[PublicAPI] TaxiStand = 91,
			[PublicAPI] TrainStation = 92,
			[PublicAPI] TravelAgency = 93,
			[PublicAPI] University = 94,
			[PublicAPI] VeterinaryCare = 95,
			[PublicAPI] Zoo = 96,
			[PublicAPI] AdministrativeAreaLevel1 = 1001,
			[PublicAPI] AdministrativeAreaLevel2 = 1002,
			[PublicAPI] AdministrativeAreaLevel3 = 1003,
			[PublicAPI] AdministrativeAreaLevel4 = 1003,
			[PublicAPI] AdministrativeAreaLevel5 = 1003,
			[PublicAPI] ColloquialArea = 1004,
			[PublicAPI] Country = 1005,
			[PublicAPI] Floor = 1006,
			[PublicAPI] Geocode = 1007,
			[PublicAPI] Intersection = 1008,
			[PublicAPI] Locality = 1009,
			[PublicAPI] NaturalFeature = 1010,
			[PublicAPI] Neighborhood = 1011,
			[PublicAPI] Political = 1012,
			[PublicAPI] PointOfInterest = 1013,
			[PublicAPI] PostBox = 1014,
			[PublicAPI] PostalCode = 1015,
			[PublicAPI] PostalCodePrefix = 1016,
			[PublicAPI] PostalTown = 1017,
			[PublicAPI] Premise = 1018,
			[PublicAPI] Room = 1019,
			[PublicAPI] Route = 1020,
			[PublicAPI] StreetAddress = 1021,
			[PublicAPI] Sublocality = 1022,
			[PublicAPI] SublocalityLevel1 = 1023,
			[PublicAPI] SublocalityLevel2 = 1024,
			[PublicAPI] SublocalityLevel3 = 1025,
			[PublicAPI] SublocalityLevel4 = 1026,
			[PublicAPI] SublocalityLevel5 = 1027,
			[PublicAPI] Subpremise = 1028,
			[PublicAPI] SyntheticGeocode = 1029,
			[PublicAPI] TransitStation = 1030
		}

		[PublicAPI]
		Place()
		{
			PlaceTypes = new List<PlaceType>();
		}

		/// <summary>
		///     Returns the unique id of this Place.
		/// </summary>
		[PublicAPI]
		public string Id { get; private set; }

		/// <summary>
		///     Returns a human readable address for this Place.
		/// </summary>
		[PublicAPI]
		public string Address { get; private set; }

		/// <summary>
		///     Returns the attributions to be shown to the user if data from the Place is used.
		/// </summary>
		[PublicAPI]
		public string Attrubutions { get; private set; }

		/// <summary>
		///     Returns the name of this Place.
		/// </summary>
		[PublicAPI]
		public string Name { get; private set; }

		/// <summary>
		///     Returns the place's phone number in international format.
		/// </summary>
		[PublicAPI]
		public string PhoneNumber { get; private set; }

		/// <summary>
		///     Returns the locale in which the names and addresses were localized.
		/// </summary>
		[PublicAPI]
		public string Locale { get; private set; }

		/// <summary>
		///     Returns a list of place types for this Place.
		/// </summary>
		[PublicAPI]
		public List<PlaceType> PlaceTypes { get; private set; }

		/// <summary>
		///     Returns the price level for this place on a scale from 0 (cheapest) to 4.
		/// </summary>
		[PublicAPI]
		public int PriceLevel { get; private set; }

		/// <summary>
		///     Returns the place's rating, from 1.0 to 5.0, based on aggregated user reviews.
		/// </summary>
		[PublicAPI]
		public float Rating { get; private set; }

		/// <summary>
		///     Returns the location of this Place.
		/// </summary>
		[PublicAPI]
		public LatLng Location { get; private set; }

		/// <summary>
		///     Returns a viewport for displaying this Place.
		/// </summary>
		[PublicAPI]
		public LatLngBounds Viewport { get; private set; }

		/// <summary>
		///     Returns the URI of the website of this Place.
		/// </summary>
		[PublicAPI]
		public string WebsiteUrl { get; private set; }

		[PublicAPI]
		public static Place FromJson(string json)
		{
			var dict = Json.Deserialize(json) as Dictionary<string, object>;
			var place = new Place();

			if (dict == null)
			{
				LogDeserializeError();
				return place;
			}

			try
			{
				place.Id = dict.GetStr("id");
				place.Address = dict.GetStr("address");
				place.Attrubutions = dict.GetStr("attributions");
				place.Name = dict.GetStr("name");
				place.PhoneNumber = dict.GetStr("phoneNumber");
				place.Locale = dict.GetStr("locale");
				var placeTypes = (List<object>) dict["placeTypes"];

				if (GoogleMapUtils.IsAndroid)
				{
					foreach (var placeType in placeTypes)
					{
						place.PlaceTypes.Add((PlaceType) (int) (long) placeType);
					}
				}
				else
				{
					foreach (var placeType in placeTypes)
					{
						place.PlaceTypes.Add(_iosPlaceMappings[placeType.ToString()]);
					}
				}

				place.PriceLevel = dict.GetInt("priceLevel");
				place.Rating = dict.GetFloat("rating");
				place.WebsiteUrl = dict.GetStr("uri");
				var lat = (double) dict["lat"];
				var lng = (double) dict["lng"];
				place.Location = new LatLng(lat, lng);

				if (dict.ContainsKey("boundsNorthEastLat") && dict.ContainsKey("boundsNorthEastLng") && dict.ContainsKey("boundsSouthWestLat") && dict.ContainsKey("boundsSouthWestLng"))
				{
					var boundsNorthEastLat = (double) dict["boundsNorthEastLat"];
					var boundsNorthEastLng = (double) dict["boundsNorthEastLng"];
					var boundsSouthWestLat = (double) dict["boundsSouthWestLat"];
					var boundsSouthWestLng = (double) dict["boundsSouthWestLng"];
					place.Viewport = new LatLngBounds(
						new LatLng(boundsSouthWestLat, boundsSouthWestLng),
						new LatLng(boundsNorthEastLat, boundsNorthEastLng));
				}
			}
			catch (Exception e)
			{
				LogDeserializeError();
				Debug.LogException(e);
			}

			return place;
		}

		static void LogDeserializeError()
		{
			Debug.LogError("Something went wrong when deserializing Place object");
		}

		[PublicAPI]
		public override string ToString()
		{
			var join = string.Join(",", PlaceTypes.ConvertAll(x => x.ToString()).ToArray());
			return string.Format(
				"[Place: Id={0}, Address={1}, Attrubutions={2}, Name={3}, PhoneNumber={4}, Locale={5}, PlaceTypes={6}, PriceLevel={7}, Rating={8}, Location={9}, Viewport={10}, WebsiteUrl={11}]",
				Id, Address,
				Attrubutions, Name, PhoneNumber, Locale, join, PriceLevel, Rating, Location, Viewport, WebsiteUrl);
		}
	}
}