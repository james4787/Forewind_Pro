using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Forewind
{
    /// <summary>
    /// 将边部顶点归拢
    /// </summary>
    public struct EdgeVertices
    {
        public Vector3 v1, v2, v3, v4;

        public EdgeVertices(Vector3 corner1, Vector3 corner2)
        {
            v1 = corner1;
            v2 = Vector3.Lerp(corner1, corner2, 1f / 3f);
            v3 = Vector3.Lerp(corner1, corner2, 2f / 3f);
            v4 = corner2;
        }

        public static EdgeVertices TerraceLerp(EdgeVertices a, EdgeVertices b, int step)
        {
            EdgeVertices result;
            result.v1 = HexMetrics.TerraceLerp(a.v1, b.v1, step);
            result.v2 = HexMetrics.TerraceLerp(a.v2, b.v2, step);
            result.v3 = HexMetrics.TerraceLerp(a.v3, b.v3, step);
            result.v4 = HexMetrics.TerraceLerp(a.v4, b.v4, step);
            return result;
        }
    }


    public class HexCell : MonoBehaviour
    {
        public HexCoordinates coordinates;

        public Color Color
        {
            get
            {
                return color;
            }
            set
            {
                if (color == value)
                {
                    return;
                }
                color = value;
                Refresh();
            }
        }

        Color color;

        int elevation = int.MinValue;

        public RectTransform uiRect;
        // 获取所属区块的引用
        public HexGridChunk chunk;

        public int Elevation
        {
            get
            {
                return elevation;
            }
            set
            {
                // 如果相等不更新网格数据
                if (elevation == value)
                {
                    return;
                }
                // 六边形高度设置
                elevation = value;
                Vector3 position = transform.localPosition;
                position.y = value * HexMetrics.elevationStep;
                position.y +=
                (HexMetrics.SampleNoise(position).y * 2f - 1f) *
                HexMetrics.elevationPerturbStrength;

                transform.localPosition = position;

                // UI高度设定，因为UI屏幕绕X轴旋转90度，所以高度沿Z轴负值增长
                Vector3 uiPosition = uiRect.localPosition;
                uiPosition.z = -position.y;
                uiRect.localPosition = uiPosition;

                // 区域刷新，更新区块网格
                Refresh();
            }
        }


        public Vector3 Position
        {
            get
            {
                return transform.localPosition;
            }
        }

        // 存储临近对象索引
        [SerializeField]
        HexCell[] neighbors;

        public HexCell GetNeighbor(HexDirection direction)
        {
            return neighbors[(int) direction];
        }

        public void SetNeighbor(HexDirection direction, HexCell cell)
        {
            neighbors[(int) direction] = cell;
            cell.neighbors[(int) direction.Opposite()] = this;
        }


        public HexEdgeType GetEdgeType(HexDirection direction)
        {
            return HexMetrics.GetEdgeType(
                elevation, neighbors[(int) direction].elevation
            );
        }

        public HexEdgeType GetEdgeType(HexCell otherCell)
        {
            return HexMetrics.GetEdgeType(
                elevation, otherCell.elevation
            );
        }

        /// <summary>
        /// 区域刷新，更新区块网格（私有方法）
        /// </summary>
        void Refresh()
        {
            // 只有当获取到引用时才刷新
            if (chunk)
            {
                chunk.Refresh();
                // 相邻的不同区块也进行刷新
                for (int i = 0; i < neighbors.Length; i++)
                {
                    HexCell neighbor = neighbors[i];
                    if (neighbor != null && neighbor.chunk != chunk)
                    {
                        neighbor.chunk.Refresh();
                    }
                }
            }
        }
    }
}