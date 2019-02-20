namespace DeadMosquito.GoogleMapsView
{
	using JetBrains.Annotations;

	/// <inheritdoc />
	/// <summary>
	/// Cap that is squared off after extending half the stroke width beyond the start or end vertex of a <see cref="T:DeadMosquito.GoogleMapsView.Polyline" /> with solid stroke pattern.
	/// See: https://developers.google.com/android/reference/com/google/android/gms/maps/model/SquareCap
	/// </summary>
	[PublicAPI]
	public sealed class SquareCap : Cap
	{
	}
}