using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace DeadMosquito.GoogleMapsView
{
    [PublicAPI]
    public interface TileProvider
    {
        AndroidJavaObject AJO { get; }
        
        Dictionary<string, object> Dictionary { get; }
    }
}