using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Forewind
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class HexMesh : MonoBehaviour
    {

        Mesh hexMesh;
        List<Vector3> vertices;
        List<int> triangles;
        List<Color> colors;

        MeshCollider meshCollider;

        void Awake()
        {
            GetComponent<MeshFilter>().mesh = hexMesh = new Mesh();
            meshCollider = gameObject.AddComponent<MeshCollider>();

            hexMesh.name = "Hex Mesh";
            vertices = new List<Vector3>();
            colors = new List<Color>();
            triangles = new List<int>();
        }

        /// <summary>
        /// 生成六边形网格
        /// </summary>
        /// <param name="cells"></param>
        public void Triangulate(HexCell[] cells)
        {
            hexMesh.Clear();
            vertices.Clear();
            colors.Clear();
            triangles.Clear();
            for (int i = 0; i < cells.Length; i++)
            {
                Triangulate(cells[i]);
            }
            hexMesh.vertices = vertices.ToArray();
            hexMesh.colors = colors.ToArray();
            hexMesh.triangles = triangles.ToArray();
            hexMesh.RecalculateNormals();

            meshCollider.sharedMesh = hexMesh;
        }

        /// <summary>
        /// 循环生成6个三角形
        /// </summary>
        /// <param name="cell"></param>
        void Triangulate(HexCell cell)
        {
            for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
            {
                Triangulate(d, cell);
            }
        }

        /// <summary>
        /// 生成1个三角形
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="cell"></param>
        void Triangulate(HexDirection direction, HexCell cell)
        {
            Vector3 center = cell.transform.localPosition;
            Vector3 v1 = center + HexMetrics.GetFirstSolidCorner(direction);
            Vector3 v2 = center + HexMetrics.GetSecondSolidCorner(direction);

            // 填充内部纯色
            AddTriangle(center, v1, v2);
            AddTriangleColor(cell.color);

            TriangulateConnection(direction, cell, v1, v2);

            if (direction == HexDirection.NE)
            {
                TriangulateConnection(direction, cell, v1, v2);
            }

            if (direction <= HexDirection.SE)
            {
                TriangulateConnection(direction, cell, v1, v2);
            }
        }

        void TriangulateConnection(HexDirection direction, HexCell cell, Vector3 v1, Vector3 v2)
        {
            HexCell neighbor = cell.GetNeighbor(direction);

            // 防止边界过分填充
            if (neighbor == null)
            {
                return;
            }

            Vector3 bridge = HexMetrics.GetBridge(direction);
            Vector3 v3 = v1 + bridge;
            Vector3 v4 = v2 + bridge;

            AddQuad(v1, v2, v3, v4);
            AddQuadColor(cell.color, neighbor.color);

            // 构造边界三角形
            HexCell nextNeighbor = cell.GetNeighbor(direction.Next());
            if (direction <= HexDirection.E && nextNeighbor != null)
            {
                AddTriangle(v2, v4, v2 + HexMetrics.GetBridge(direction.Next()));
                AddTriangleColor(cell.color, neighbor.color, nextNeighbor.color);
            }
        }

        /// <summary>
        /// 添加梯形顶点及三角形索引
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        /// <param name="v4"></param>
        void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
        {
            int vertexIndex = vertices.Count;
            vertices.Add(v1);
            vertices.Add(v2);
            vertices.Add(v3);
            vertices.Add(v4);
            triangles.Add(vertexIndex);
            triangles.Add(vertexIndex + 2);
            triangles.Add(vertexIndex + 1);
            triangles.Add(vertexIndex + 1);
            triangles.Add(vertexIndex + 2);
            triangles.Add(vertexIndex + 3);
        }

        void AddQuadColor(Color c1, Color c2, Color c3, Color c4)
        {
            colors.Add(c1);
            colors.Add(c2);
            colors.Add(c3);
            colors.Add(c4);
        }

        void AddQuadColor(Color c1, Color c2)
        {
            colors.Add(c1);
            colors.Add(c1);
            colors.Add(c2);
            colors.Add(c2);
        }

        /// <summary>
        /// 添加各个顶点颜色
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <param name="c3"></param>
        void AddTriangleColor(Color c1, Color c2, Color c3)
        {
            colors.Add(c1);
            colors.Add(c2);
            colors.Add(c3);
        }

        void AddTriangleColor(Color color)
        {
            colors.Add(color);
            colors.Add(color);
            colors.Add(color);
        }
        /// <summary>
        /// 添加三角形顶点
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            int vertexIndex = vertices.Count;
            vertices.Add(v1);
            vertices.Add(v2);
            vertices.Add(v3);
            triangles.Add(vertexIndex);
            triangles.Add(vertexIndex + 1);
            triangles.Add(vertexIndex + 2);
        }
    }
}