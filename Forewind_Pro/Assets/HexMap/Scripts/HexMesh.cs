using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Forewind
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class HexMesh : MonoBehaviour
    {

        Mesh hexMesh;
        static List<Vector3> vertices = new List<Vector3>();
        static List<Color> colors = new List<Color>();
        static List<int> triangles = new List<int>();

        MeshCollider meshCollider;

        void Awake()
        {
            GetComponent<MeshFilter>().mesh = hexMesh = new Mesh();
            meshCollider = gameObject.AddComponent<MeshCollider>();
            hexMesh.name = "Hex Mesh";
        }

        /// <summary>
        /// 添加梯形顶点及三角形索引
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        /// <param name="v4"></param>
        public void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
        {
            int vertexIndex = vertices.Count;
            // 添加顶点扰动
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

        public void AddQuadColor(Color c1, Color c2, Color c3, Color c4)
        {
            colors.Add(c1);
            colors.Add(c2);
            colors.Add(c3);
            colors.Add(c4);
        }

        public void AddQuadColor(Color c1, Color c2)
        {
            colors.Add(c1);
            colors.Add(c1);
            colors.Add(c2);
            colors.Add(c2);
        }

        public void AddQuadColor(Color color)
        {
            colors.Add(color);
            colors.Add(color);
            colors.Add(color);
            colors.Add(color);
        }
        /// <summary>
        /// 添加各个顶点颜色
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <param name="c3"></param>
        public void AddTriangleColor(Color c1, Color c2, Color c3)
        {
            colors.Add(c1);
            colors.Add(c2);
            colors.Add(c3);
        }

        public void AddTriangleColor(Color color)
        {
            colors.Add(color);
            colors.Add(color);
            colors.Add(color);
        }
        /// <summary>
        /// 添加三角形顶点，已添加扰动
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        public void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            int vertexIndex = vertices.Count;
            // 添加顶点扰动
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
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
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

        public void Clear()
        {
            hexMesh.Clear();
            vertices.Clear();
            colors.Clear();
            triangles.Clear();
        }

        public void Apply()
        {
            hexMesh.SetVertices(vertices);
            hexMesh.SetColors(colors);
            hexMesh.SetTriangles(triangles, 0);
            hexMesh.RecalculateNormals();
            meshCollider.sharedMesh = hexMesh;
        }
    }
}