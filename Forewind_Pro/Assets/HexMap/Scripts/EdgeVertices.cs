using UnityEngine;

public struct EdgeVertices {

	public Vector3 v1, v2, v3, v4, v5;

    /// <summary>
    /// 边界均匀四等分插值
    /// </summary>
    /// <param name="corner1">边界顶点一</param>
    /// <param name="corner2">边界顶点二</param>
	public EdgeVertices (Vector3 corner1, Vector3 corner2) {
		v1 = corner1;
		v2 = Vector3.Lerp(corner1, corner2, 0.25f);
		v3 = Vector3.Lerp(corner1, corner2, 0.5f);
		v4 = Vector3.Lerp(corner1, corner2, 0.75f);
		v5 = corner2;
	}

    /// <summary>
    /// 边界两侧根据插值数量进行插值
    /// </summary>
    /// <param name="corner1">边界顶点一</param>
    /// <param name="corner2">边界顶点二</param>
    /// <param name="outerStep">插值比例</param>
	public EdgeVertices (Vector3 corner1, Vector3 corner2, float outerStep) {
		v1 = corner1;
		v2 = Vector3.Lerp(corner1, corner2, outerStep);
		v3 = Vector3.Lerp(corner1, corner2, 0.5f);
		v4 = Vector3.Lerp(corner1, corner2, 1f - outerStep);
		v5 = corner2;
	}

    /// <summary>
    /// 斜坡顶点插值
    /// </summary>
    /// <param name="a">边界顶点一</param>
    /// <param name="b">边界顶点二</param>
    /// <param name="step">插值数量</param>
    /// <returns></returns>
	public static EdgeVertices TerraceLerp (
		EdgeVertices a, EdgeVertices b, int step)
	{
		EdgeVertices result;
		result.v1 = HexMetrics.TerraceLerp(a.v1, b.v1, step);
		result.v2 = HexMetrics.TerraceLerp(a.v2, b.v2, step);
		result.v3 = HexMetrics.TerraceLerp(a.v3, b.v3, step);
		result.v4 = HexMetrics.TerraceLerp(a.v4, b.v4, step);
		result.v5 = HexMetrics.TerraceLerp(a.v5, b.v5, step);
		return result;
	}
}