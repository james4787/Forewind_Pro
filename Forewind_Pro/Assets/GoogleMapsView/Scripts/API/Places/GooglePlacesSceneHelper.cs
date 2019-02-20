
using DeadMosquito.GoogleMapsView;

namespace DeadMosquito.GoogleMapsView
{
	using System;
	using System.Collections.Generic;
	using JetBrains.Annotations;
	using UnityEngine;

	class GooglePlacesSceneHelper : MonoBehaviour
	{
		static GooglePlacesSceneHelper _instance;
		static readonly object InitLock = new object();
		readonly object _queueLock = new object();
		readonly List<Action> _queuedActions = new List<Action>();
		readonly List<Action> _executingActions = new List<Action>();

		public static GooglePlacesSceneHelper Instance
		{
			get
			{
				if (_instance == null)
				{
					Init();
				}
				return _instance;
			}
		}

		internal static void Init()
		{
			lock (InitLock)
			{
				if (ReferenceEquals(_instance, null))
				{
					var instances = FindObjectsOfType<GooglePlacesSceneHelper>();

					var className = typeof(GooglePlacesSceneHelper).Name;
					if (instances.Length > 1)
					{
						Debug.LogError(className + " Something went really wrong " +
						               " - there should never be more than 1 " + className +
						               " Reopening the scene might fix it.");
					}
					else if (instances.Length == 0)
					{
						var singleton = new GameObject();
						_instance = singleton.AddComponent<GooglePlacesSceneHelper>();
						singleton.name = className;

						DontDestroyOnLoad(singleton);

						Debug.Log("[Singleton] An _instance of " + className +
						          " is needed in the scene, so '" + singleton.name +
						          "' was created with DontDestroyOnLoad.");
					}
					else
					{
						Debug.Log("[Singleton] Using _instance already created: " + _instance.gameObject.name);
					}
				}
			}
		}

		GooglePlacesSceneHelper()
		{
		}

		internal static void Queue(Action action)
		{
			if (action == null)
			{
				Debug.LogWarning("Trying to queue null action");
				return;
			}

			lock (_instance._queueLock)
			{
				_instance._queuedActions.Add(action);
			}
		}

		void Update()
		{
			MoveQueuedActionsToExecuting();

			while (_executingActions.Count > 0)
			{
				var action = _executingActions[0];
				_executingActions.RemoveAt(0);
				action();
			}
		}

		void MoveQueuedActionsToExecuting()
		{
			lock (_queueLock)
			{
				while (_queuedActions.Count > 0)
				{
					var action = _queuedActions[0];
					_executingActions.Add(action);
					_queuedActions.RemoveAt(0);
				}
			}
		}

		// These are invoked from Java by UnityPlayer.UnitySendMessage

		#region callbacks

		[UsedImplicitly]
		public void OnPlacePickSuccess(string placeJson)
		{
			if (Debug.isDebugBuild)
			{
				Debug.Log("Place was picked successfully: " + placeJson);
			}
			PlacePicker.OnSuccess(placeJson);
		}

		[UsedImplicitly]
		public void OnPlacePickFailed(string message)
		{
			if (Debug.isDebugBuild)
			{
				Debug.Log("Place picking failed: " + message);
			}
			PlacePicker.OnFailure(message);
		}
		
		[UsedImplicitly]
		public void OnPlaceAutocompleteSuccess(string placeJson)
		{
			if (Debug.isDebugBuild)
			{
				Debug.Log("Place was picked successfully: " + placeJson);
			}
			PlaceAutocomplete.OnSuccess(placeJson);
		}

		[UsedImplicitly]
		public void OnPlaceAutocompleteFailed(string message)
		{
			if (Debug.isDebugBuild)
			{
				Debug.Log("Place picking failed: " + message);
			}
			PlaceAutocomplete.OnFailure(message);
		}

		#endregion
	}
}