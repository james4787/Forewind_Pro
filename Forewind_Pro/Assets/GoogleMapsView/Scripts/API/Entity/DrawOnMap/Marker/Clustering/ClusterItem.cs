using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace DeadMosquito.GoogleMapsView
{
	/// <summary>
	/// Item to be added to the cluster
	/// </summary>
	[PublicAPI]
	public class ClusterItem
	{
		const string ClusteredMarkerClass = "com.deadmosquitogames.gmaps.clustering.MyClusterItem";

		/// <summary>
		/// Position of the item
		/// </summary>
		[PublicAPI]
		public LatLng Position { get; private set; }

		/// <summary>
		/// Title of the item
		/// </summary>
		[PublicAPI]
		public string Title { get; private set; }

		/// <summary>
		/// Snippet of the item
		/// </summary>
		[PublicAPI]
		public string Snippet { get; private set; }

		[PublicAPI]
		public ClusterItem(LatLng position, string title, string snippet)
		{
			Position = position;
			Title = title;
			Snippet = snippet;
		}

		[PublicAPI]
		public ClusterItem(LatLng position)
		{
			Position = position;
		}

		public AndroidJavaObject ToAJO()
		{
			return new AndroidJavaObject(ClusteredMarkerClass, Position.ToAJO(), Title, Snippet);
		}

		public Dictionary<string, object> ToDictionary()
		{
			var result = new Dictionary<string, object>();
			result["lat"] = Position.Latitude;
			result["lng"] = Position.Longitude;
			result["title"] = Title;
			result["snippet"] = Snippet;
			return result;
		}
	}
}