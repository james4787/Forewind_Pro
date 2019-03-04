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
        public Vector3 v1, v2, v3, v4, v5;

        public EdgeVertices(Vector3 corner1, Vector3 corner2)
        {
            v1 = corner1;
            v2 = Vector3.Lerp(corner1, corner2, 0.25f);
            v3 = Vector3.Lerp(corner1, corner2, 0.5f);
            v4 = Vector3.Lerp(corner1, corner2, 0.75f);
            v5 = corner2;
        }

        public EdgeVertices(Vector3 corner1, Vector3 corner2, float outerStep)
        {
            v1 = corner1;
            v2 = Vector3.Lerp(corner1, corner2, outerStep);
            v3 = Vector3.Lerp(corner1, corner2, 0.5f);
            v4 = Vector3.Lerp(corner1, corner2, 1f - outerStep);
            v5 = corner2;
        }

        public static EdgeVertices TerraceLerp(EdgeVertices a, EdgeVertices b, int step)
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

                if (hasOutgoingRiver && elevation < GetNeighbor(outgoingRiver).elevation)
                {
                    RemoveOutgoingRiver();
                }
                if (hasIncomingRiver && elevation > GetNeighbor(incomingRiver).elevation)
                {
                    RemoveIncomingRiver();
                }
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

        /// <summary>
        /// 获取河床高度
        /// </summary>
        public float StreamBedY
        {
            get
            {
                return
                    (elevation + HexMetrics.streamBedElevationOffset) *
                    HexMetrics.elevationStep;
            }
        }

        // 河流使用
        bool hasIncomingRiver, hasOutgoingRiver;
        HexDirection incomingRiver, outgoingRiver;

        public bool HasIncomingRiver
        {
            get
            {
                return hasIncomingRiver;
            }
        }

        public bool HasOutgoingRiver
        {
            get
            {
                return hasOutgoingRiver;
            }
        }

        public HexDirection IncomingRiver
        {
            get
            {
                return incomingRiver;
            }
        }

        public HexDirection OutgoingRiver
        {
            get
            {
                return outgoingRiver;
            }
        }

        public bool HasRiver
        {
            get
            {
                return hasIncomingRiver || hasOutgoingRiver;
            }
        }

        public bool HasRiverBeginOrEnd
        {
            get
            {
                return hasIncomingRiver != hasOutgoingRiver;
            }
        }

        public bool HasRiverThroughEdge(HexDirection direction)
        {
            return
                hasIncomingRiver && incomingRiver == direction ||
                hasOutgoingRiver && outgoingRiver == direction;
        }

        /// <summary>
        /// 移除流出河流
        /// </summary>
        public void RemoveOutgoingRiver()
        {
            if (!hasOutgoingRiver)
            {
                return;
            }
            hasOutgoingRiver = false;
            RefreshSelfOnly();

            HexCell neighbor = GetNeighbor(outgoingRiver);
            neighbor.hasIncomingRiver = false;
            neighbor.RefreshSelfOnly();
        }

        /// <summary>
        /// 只刷新自身区块，用于移除河流
        /// </summary>
        void RefreshSelfOnly()
        {
            chunk.Refresh();
        }

        /// <summary>
        /// 移除流入河流
        /// </summary>
        public void RemoveIncomingRiver()
        {
            if (!hasIncomingRiver)
            {
                return;
            }
            hasIncomingRiver = false;
            RefreshSelfOnly();

            HexCell neighbor = GetNeighbor(incomingRiver);
            neighbor.hasOutgoingRiver = false;
            neighbor.RefreshSelfOnly();
        }

        /// <summary>
        /// 移除全部河流
        /// </summary>
        public void RemoveRiver()
        {
            RemoveOutgoingRiver();
            RemoveIncomingRiver();
        }

        /// <summary>
        /// 添加河流
        /// </summary>
        /// <param name="direction"></param>
        public void SetOutgoingRiver(HexDirection direction)
        {
            if (hasOutgoingRiver && outgoingRiver == direction)
            {
                return;
            }
            // 确保流出方向的对象不为空，且高度差为正进行向下流动
            HexCell neighbor = GetNeighbor(direction);
            if (!neighbor || elevation < neighbor.elevation)
            {
                return;
            }
            // 移除之前的流出或与设定方向一致的流入河流
            RemoveOutgoingRiver();
            if (hasIncomingRiver && incomingRiver == direction)
            {
                RemoveIncomingRiver();
            }
            // 设置流出河流
            hasOutgoingRiver = true;
            outgoingRiver = direction;
            RefreshSelfOnly();
            // 同时将相邻的设为流入河流
            if (neighbor.hasIncomingRiver && neighbor.incomingRiver == direction.Opposite())
            {
                RemoveIncomingRiver();
            }
            else
            {
                neighbor.hasIncomingRiver = true;
            }
            neighbor.incomingRiver = direction.Opposite();
            neighbor.RefreshSelfOnly();

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