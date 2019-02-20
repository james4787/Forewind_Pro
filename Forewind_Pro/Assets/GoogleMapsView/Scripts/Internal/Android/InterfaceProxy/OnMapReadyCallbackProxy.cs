// ReSharper disable InconsistentNaming
namespace DeadMosquito.GoogleMapsView.Internal
{
	using System;
	using JetBrains.Annotations;
	using UnityEngine;

	public sealed class OnMapReadyCallbackProxy : AndroidJavaProxy
	{
		readonly Action _onMapReady;

		public OnMapReadyCallbackProxy(Action onMapReady)
			: base("com.google.android.gms.maps.OnMapReadyCallback")
		{
			_onMapReady = onMapReady;
		}

		[UsedImplicitly]
		public void onMapReady(AndroidJavaObject map)
		{
			GoogleMapsSceneHelper.Queue(_onMapReady);
		}
	}
}
