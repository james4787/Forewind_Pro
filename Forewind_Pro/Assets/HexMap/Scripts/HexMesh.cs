using UnityEngine;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexMesh : MonoBehaviour
{

    public bool useCollider, useColors, useUVCoordinates;

    [NonSerialized] List<Vector3> vertices;
    [NonSerialized] List<Color> colors;
    [NonSerialized] List<Vector2> uvs;
    [NonSerialized] List<int> triangles;

    Mesh hexMesh;
    MeshCollider meshCollider;

    void Awake()
    {
        GetComponent<MeshFilter>().mesh = hexMesh = new Mesh();
        // 对于河流不需要碰撞器，所以进行判断
        if (useCollider)
        {
            meshCollider = gameObject.AddComponent<MeshCollider>();
        }
        hexMesh.name = "Hex Mesh";
    }

    public void Clear()
    {
        hexMesh.Clear();
        vertices = ListPool<Vector3>.Get();
        // 河流颜色使用噪音贴图
        if (useColors)
        {
            colors = ListPool<Color>.Get();
        }
        // 地形不需要UV坐标，只有河流用得到
        if (useUVCoordinates)
        {
            uvs = ListPool<Vector2>.Get();
        }
        triangles = ListPool<int>.Get();
    }

    public void Apply()
    {
        // 在应用网格数据后我们不再需要它们，因此我们可以将它们添加到池中
        hexMesh.SetVertices(vertices);
        ListPool<Vector3>.Add(vertices);
        // 河流颜色使用噪音贴图
        if (useColors)
        {
            hexMesh.SetColors(colors);
            ListPool<Color>.Add(colors);
        }
        // 地形不需要UV坐标，只有河流用得到
        if (useUVCoordinates)
        {
            hexMesh.SetUVs(0, uvs);
            ListPool<Vector2>.Add(uvs);
        }
        hexMesh.SetTriangles(triangles, 0);
        ListPool<int>.Add(triangles);
        hexMesh.RecalculateNormals();
        // 河流不需要碰撞器
        if (useCollider)
        {
            meshCollider.sharedMesh = hexMesh;
        }
    }

    /// <summary>
    /// 添加三角形顶点，已添加扰动
    /// </summary>
    /// <param name="v1">顶点一</param>
    /// <param name="v2">顶点二</param>
    /// <param name="v3">顶点三</param>
    public void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        int vertexIndex = vertices.Count;
        vertices.Add(HexMetrics.Perturb(v1));
        vertices.Add(HexMetrics.Perturb(v2));
        vertices.Add(HexMetrics.Perturb(v3));
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
    }

    /// <summary>
    /// 无噪音扰动三角化
    /// </summary>
    /// <param name="v1">顶点一</param>
    /// <param name="v2">顶点二</param>
    /// <param name="v3">顶点三</param>
    public void AddTriangleUnperturbed(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        int vertexIndex = vertices.Count;
        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
    }

    /// <summary>
    /// 添加各个顶点颜色
    /// </summary>
    /// <param name="color">顶点颜色</param>
    public void AddTriangleColor(Color color)
    {
        colors.Add(color);
        colors.Add(color);
        colors.Add(color);
    }

    /// <summary>
    /// 添加各个顶点颜色
    /// </summary>
    /// <param name="c1">颜色一</param>
    /// <param name="c2">颜色二</param>
    /// <param name="c3">颜色三</param>
    public void AddTriangleColor(Color c1, Color c2, Color c3)
    {
        colors.Add(c1);
        colors.Add(c2);
        colors.Add(c3);
    }

    /// <summary>
    /// 添加三角化UV坐标
    /// </summary>
    /// <param name="uv1">uv1</param>
    /// <param name="uv2">uv2</param>
    /// <param name="uv3">uv3</param>
    public void AddTriangleUV(Vector2 uv1, Vector2 uv2, Vector3 uv3)
    {
        uvs.Add(uv1);
        uvs.Add(uv2);
        uvs.Add(uv3);
    }

    /// <summary>
    /// 添加梯形顶点及三角形索引
    /// </summary>
    /// <param name="v1">顶点一</param>
    /// <param name="v2">顶点二</param>
    /// <param name="v3">顶点三</param>
    /// <param name="v4">顶点四</param>
    public void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
    {
        int vertexIndex = vertices.Count;
        vertices.Add(HexMetrics.Perturb(v1));
        vertices.Add(HexMetrics.Perturb(v2));
        vertices.Add(HexMetrics.Perturb(v3));
        vertices.Add(HexMetrics.Perturb(v4));
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 3);
    }
    
    /// <summary>
    /// 添加四边形各个顶点颜色
    /// </summary>
    /// <param name="color">顶点颜色</param>
    public void AddQuadColor(Color color)
    {
        colors.Add(color);
        colors.Add(color);
        colors.Add(color);
        colors.Add(color);
    }

    /// <summary>
    /// 添加四边形各个顶点颜色
    /// </summary>
    /// <param name="c1">内侧颜色</param>
    /// <param name="c2">外侧颜色</param>
    public void AddQuadColor(Color c1, Color c2)
    {
        colors.Add(c1);
        colors.Add(c1);
        colors.Add(c2);
        colors.Add(c2);
    }

    /// <summary>
    /// 添加四边形各个顶点颜色
    /// </summary>
    /// <param name="v1">顶点一颜色</param>
    /// <param name="v2">顶点二颜色</param>
    /// <param name="v3">顶点三颜色</param>
    /// <param name="v4">顶点四颜色</param>
    public void AddQuadColor(Color c1, Color c2, Color c3, Color c4)
    {
        colors.Add(c1);
        colors.Add(c2);
        colors.Add(c3);
        colors.Add(c4);
    }

    /// <summary>
    /// 添加四边形UV坐标
    /// </summary>
    /// <param name="uv1">uv1向量</param>
    /// <param name="uv2">uv2向量</param>
    /// <param name="uv3">uv3向量</param>
    /// <param name="uv4">uv3向量</param>
    public void AddQuadUV(Vector2 uv1, Vector2 uv2, Vector3 uv3, Vector3 uv4)
    {
        uvs.Add(uv1);
        uvs.Add(uv2);
        uvs.Add(uv3);
        uvs.Add(uv4);
    }

    /// <summary>
    /// 添加四边形UV坐标的另一种重载
    /// </summary>
    /// <param name="uMin">u最小</param>
    /// <param name="uMax">u最大</param>
    /// <param name="vMin">v最小</param>
    /// <param name="vMax">v最大</param>
    public void AddQuadUV(float uMin, float uMax, float vMin, float vMax)
    {
        uvs.Add(new Vector2(uMin, vMin));
        uvs.Add(new Vector2(uMax, vMin));
        uvs.Add(new Vector2(uMin, vMax));
        uvs.Add(new Vector2(uMax, vMax));
    }
}