using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DeadMosquito.GoogleMapsView.Internal;
using DeadMosquito.GoogleMapsView.MiniJSON;
using DeadMosquito.JniToolkit;
using JetBrains.Annotations;
using UnityEngine;

namespace DeadMosquito.GoogleMapsView
{
	/// <summary>
	/// Class to create and manage clusters of markers
	/// </summary>
	[PublicAPI]
	public sealed class ClusterManager
	{
		const string ClusterManagerClass = "com.google.maps.android.clustering.ClusterManager";

		readonly GoogleMapsView _googleMapsView;

		AndroidJavaObject _clusterManagerAJO;
#pragma warning disable 0414
		IntPtr _clusterManagerPtr = IntPtr.Zero;
#pragma warning restore 0414

		/// <summary>
		/// Creates new cluster manager for existing map view.
		/// </summary>
		/// <param name="googleMapsView">Map view</param>
		/// <exception cref="ArgumentNullException">Thrown if map view is null</exception>
		[PublicAPI]
		public ClusterManager([NotNull] GoogleMapsView googleMapsView)
		{
			if (GoogleMapUtils.IsPlatformNotSupported)
			{
				return;
			}

			if (googleMapsView == null)
			{
				throw new ArgumentNullException("googleMapsView");
			}

			_googleMapsView = googleMapsView;

			if (GoogleMapUtils.IsAndroid)
			{
				JniToolkitUtils.RunOnUiThread(() =>
				{
					_clusterManagerAJO = new AndroidJavaObject(ClusterManagerClass, JniToolkitUtils.Activity, _googleMapsView.GoogleMapAJO);
					_googleMapsView.GoogleMapAJO.Call("setOnCameraIdleListener", _clusterManagerAJO);
				});
			}

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
			_clusterManagerPtr = _createGoogleMapsViewClusterManager(googleMapsView.MapPtr);
#endif
		}

		/// <summary>
		/// Add item to the cluster manager.
		/// </summary>
		/// <param name="clusterItem">Item to add to the cluster. Must not be null.</param>
		/// <exception cref="ArgumentNullException">Thrown if item is null.</exception>
		[PublicAPI]
		public void AddItem([NotNull] ClusterItem clusterItem)
		{
			if (clusterItem == null)
			{
				throw new ArgumentNullException("clusterItem");
			}

			if (GoogleMapUtils.IsPlatformNotSupported)
			{
				return;
			}

			if (GoogleMapUtils.IsAndroid)
			{
				JniToolkitUtils.RunOnUiThread(() =>
				{
					_clusterManagerAJO.Call("addItem", clusterItem.ToAJO());
					AndroidCluster();
				});
			}

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
			_googleMapsViewClusterManagerAddSingleItem(_clusterManagerPtr, Json.Serialize(clusterItem.ToDictionary()));
#endif
		}

		/// <summary>
		/// Add a list of cluster items to the cluster
		/// </summary>
		/// <param name="clusterItems">Items to add. Must not be null or empty.</param>
		/// <exception cref="ArgumentNullException">Thrown if collection is null</exception>
		/// <exception cref="ArgumentException">Thrown if collection is empty</exception>
		[PublicAPI]
		public void AddItems([NotNull] List<ClusterItem> clusterItems)
		{
			if (clusterItems == null)
			{
				throw new ArgumentNullException("clusterItems");
			}

			if (clusterItems.Count == 0)
			{
				throw new ArgumentException("Value cannot be an empty collection.", "clusterItems");
			}

			if (GoogleMapUtils.IsPlatformNotSupported)
			{
				return;
			}

			if (GoogleMapUtils.IsAndroid)
			{
				JniToolkitUtils.RunOnUiThread(() =>
				{
					var ajoItems = clusterItems.ToJavaList(x => new ClusterItem(x.Position, x.Title, x.Snippet).ToAJO());
					_clusterManagerAJO.Call("addItems", ajoItems);
					AndroidCluster();
				});
			}

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
			var itemsToSerialize = clusterItems.ConvertAll(item => item.ToDictionary());
			_googleMapsViewClusterManagerAddItems(_clusterManagerPtr, Json.Serialize(itemsToSerialize));
#endif
		}

		/// <summary>
		/// Remove all the items from the cluster.
		/// </summary>
		[PublicAPI]
		public void ClearItems()
		{
			if (GoogleMapUtils.IsPlatformNotSupported)
			{
				return;
			}

			if (GoogleMapUtils.IsAndroid)
			{
				JniToolkitUtils.RunOnUiThread(() =>
				{
					_clusterManagerAJO.Call("clearItems");
					AndroidCluster();
				});
			}

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
			_googleMapsViewClusterManagerClearItems(_clusterManagerPtr);
#endif
		}

		void AndroidCluster()
		{
			_clusterManagerAJO.Call("cluster");
		}

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
		[DllImport("__Internal")]
		static extern IntPtr _createGoogleMapsViewClusterManager(IntPtr mapPtr);
		
		[DllImport("__Internal")]
		static extern void _googleMapsViewClusterManagerAddSingleItem(IntPtr managerPtr, string item);
		
		[DllImport("__Internal")]
		static extern void _googleMapsViewClusterManagerAddItems(IntPtr managerPtr, string items);
		
		[DllImport("__Internal")]
		static extern void _googleMapsViewClusterManagerClearItems(IntPtr managerPtr);
#endif
	}
}