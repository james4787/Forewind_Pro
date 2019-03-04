﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Forewind
{
    public class HexGridChunk : MonoBehaviour
    {
        HexCell[] cells;

        public HexMesh terrain;
        Canvas gridCanvas;

        void Awake()
        {
            gridCanvas = GetComponentInChildren<Canvas>();

            cells = new HexCell[HexMetrics.chunkSizeX * HexMetrics.chunkSizeZ];
            ShowUI(false);
        }

        /// <summary>
        /// Toggle 显示UI注册事件
        /// </summary>
        /// <param name="visible"></param>
        public void ShowUI(bool visible)
        {
            gridCanvas.gameObject.SetActive(visible);
        }

        public void Refresh()
        {
            enabled = true;
        }

        /// <summary>
        /// 等待所有状态更新完后，再进行刷新
        /// </summary>
        void LateUpdate()
        {
            Triangulate();
            enabled = false;
        }


        public void AddCell(int index, HexCell cell)
        {
            cells[index] = cell;
            cell.chunk = this;
            cell.transform.SetParent(transform, false);
            cell.uiRect.SetParent(gridCanvas.transform, false);
        }

        /// <summary>
        /// 生成六边形网格
        /// </summary>
        public void Triangulate()
        {
            terrain.Clear();
            for (int i = 0; i < cells.Length; i++)
            {
                Triangulate(cells[i]);
            }
            terrain.Apply();
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
            Vector3 center = cell.Position;
            EdgeVertices e = new EdgeVertices(center + HexMetrics.GetFirstSolidCorner(direction),
                                              center + HexMetrics.GetSecondSolidCorner(direction));

            // 设置平面河床高度
            if (cell.HasRiver)
            {
                if (cell.HasRiverThroughEdge(direction))
                {
                    e.v3.y = cell.StreamBedY;
                    // 三角化河道的开始和结束
                    if (cell.HasRiverBeginOrEnd)
                    {
                        TriangulateWithRiverBeginOrEnd(direction, cell, center, e);
                    }
                    else
                    {
                        TriangulateWithRiver(direction, cell, center, e);
                    }
                }
                else
                {
                    TriangulateAdjacentToRiver(direction, cell, center, e);
                }
            }
            else
            {
                TriangulateEdgeFan(center, e, cell.Color);
            }

            if (direction <= HexDirection.SE)
            {
                TriangulateConnection(direction, cell, e);
            }
        }


        void TriangulateAdjacentToRiver(HexDirection direction, HexCell cell, Vector3 center, EdgeVertices e)
        {
            if (cell.HasRiverThroughEdge(direction.Next()))
            {
                if (cell.HasRiverThroughEdge(direction.Previous()))
                {
                    center += HexMetrics.GetSolidEdgeMiddle(direction) *
                        (HexMetrics.innerToOuter * 0.5f);
                }
                else if (cell.HasRiverThroughEdge(direction.Previous2()))
                {
                    center += HexMetrics.GetFirstSolidCorner(direction) * 0.25f;
                }
            }
            else if (cell.HasRiverThroughEdge(direction.Previous()) && cell.HasRiverThroughEdge(direction.Next2()))
            {
                center += HexMetrics.GetSecondSolidCorner(direction) * 0.25f;
            }

            EdgeVertices m = new EdgeVertices(
                Vector3.Lerp(center, e.v1, 0.5f),
                Vector3.Lerp(center, e.v5, 0.5f)
            );

            TriangulateEdgeStrip(m, cell.Color, e, cell.Color);
            TriangulateEdgeFan(center, m, cell.Color);
        }

        /// <summary>
        /// 针对河流开始和结束的条件下构建网格
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="cell"></param>
        /// <param name="center"></param>
        /// <param name="e"></param>
        void TriangulateWithRiverBeginOrEnd(HexDirection direction, HexCell cell, Vector3 center, EdgeVertices e)
        {
            EdgeVertices m = new EdgeVertices(
                Vector3.Lerp(center, e.v1, 0.5f),
                Vector3.Lerp(center, e.v5, 0.5f)
            );

            m.v3.y = e.v3.y;

            TriangulateEdgeStrip(m, cell.Color, e, cell.Color);
            TriangulateEdgeFan(center, m, cell.Color);
        }

        /// <summary>
        /// 生成有河流的三角网格
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="cell"></param>
        /// <param name="center"></param>
        /// <param name="e"></param>
        void TriangulateWithRiver(HexDirection direction, HexCell cell, Vector3 center, EdgeVertices e)
        {
            Vector3 centerL, centerR;
            //
            if (cell.HasRiverThroughEdge(direction.Opposite()))
            {
                // 河道拓宽为直线的左边界点
                centerL = center + HexMetrics.GetFirstSolidCorner(direction.Previous()) * 0.25f;
                centerR = center + HexMetrics.GetSecondSolidCorner(direction.Next()) * 0.25f;
            }
            else if (cell.HasRiverThroughEdge(direction.Next()))
            {
                centerL = center;
                centerR = Vector3.Lerp(center, e.v5, 2f / 3f);
            }
            else if (cell.HasRiverThroughEdge(direction.Previous()))
            {
                centerL = Vector3.Lerp(center, e.v1, 2f / 3f);
                centerR = center;
            }
            else if (cell.HasRiverThroughEdge(direction.Next2()))
            {
                centerL = center;
                centerR = center + HexMetrics.GetSolidEdgeMiddle(direction.Next()) * (0.5f * HexMetrics.innerToOuter); ;
            }
            else // Previous2
            {
                centerL = center + HexMetrics.GetSolidEdgeMiddle(direction.Previous()) * (0.5f * HexMetrics.innerToOuter); ;
                centerR = center;
            }
            center = Vector3.Lerp(centerL, centerR, 0.5f);

            EdgeVertices m = new EdgeVertices(Vector3.Lerp(centerL, e.v1, 0.5f), Vector3.Lerp(centerR, e.v5, 0.5f), 1f / 6f);
            // 同步河道底部高度
            m.v3.y = center.y = e.v3.y;
            // 填充插值中间点与边界间的平面
            TriangulateEdgeStrip(m, cell.Color, e, cell.Color);

            // 添加河道边接近中心的三角形以及方形平面
            terrain.AddTriangle(centerL, m.v1, m.v2);
            terrain.AddTriangleColor(cell.Color);

            terrain.AddQuad(centerL, center, m.v2, m.v3);
            terrain.AddQuadColor(cell.Color);
            terrain.AddQuad(center, centerR, m.v3, m.v4);
            terrain.AddQuadColor(cell.Color);

            terrain.AddTriangle(centerR, m.v4, m.v5);
            terrain.AddTriangleColor(cell.Color);
        }

        /// <summary>
        /// 三角化连接处
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="cell"></param>
        /// <param name="e1"></param>
        void TriangulateConnection(HexDirection direction, HexCell cell, EdgeVertices e1)
        {
            HexCell neighbor = cell.GetNeighbor(direction);

            // 防止边界过分填充
            if (neighbor == null)
            {
                return;
            }

            Vector3 bridge = HexMetrics.GetBridge(direction);
            bridge.y = neighbor.Position.y - cell.Position.y;

            EdgeVertices e2 = new EdgeVertices(e1.v1 + bridge, e1.v5 + bridge);

            // 设置边界河床高度
            if (cell.HasRiverThroughEdge(direction))
            {
                e2.v3.y = neighbor.StreamBedY;
            }

            // 限制坡度的细分阶梯数
            if (cell.GetEdgeType(direction) == HexEdgeType.Slope)
            {
                TriangulateEdgeTerraces(e1, cell, e2, neighbor);
            }
            else
            {
                TriangulateEdgeStrip(e1, cell.Color, e2, neighbor.Color);
            }

            // 构造边界三角形
            HexCell nextNeighbor = cell.GetNeighbor(direction.Next());
            if (direction <= HexDirection.E && nextNeighbor != null)
            {
                Vector3 v5 = e1.v5 + HexMetrics.GetBridge(direction.Next());
                v5.y = nextNeighbor.Position.y;

                if (cell.Elevation <= neighbor.Elevation)
                {
                    if (cell.Elevation <= nextNeighbor.Elevation)
                    {
                        TriangulateCorner(e1.v5, cell, e2.v5, neighbor, v5, nextNeighbor);
                    }
                    else
                    {
                        TriangulateCorner(v5, nextNeighbor, e1.v5, cell, e2.v5, neighbor);
                    }
                }
                else if (neighbor.Elevation <= nextNeighbor.Elevation)
                {
                    TriangulateCorner(e2.v5, neighbor, v5, nextNeighbor, e1.v5, cell);
                }
                else
                {
                    TriangulateCorner(v5, nextNeighbor, e1.v5, cell, e2.v5, neighbor);
                }
            }
        }

        /// <summary>
        /// 三角化边界扇形
        /// </summary>
        /// <param name="center"></param>
        /// <param name="edge"></param>
        /// <param name="color"></param>
        void TriangulateEdgeFan(Vector3 center, EdgeVertices edge, Color color)
        {
            terrain.AddTriangle(center, edge.v1, edge.v2);
            terrain.AddTriangleColor(color);
            terrain.AddTriangle(center, edge.v2, edge.v3);
            terrain.AddTriangleColor(color);
            terrain.AddTriangle(center, edge.v3, edge.v4);
            terrain.AddTriangleColor(color);
            terrain.AddTriangle(center, edge.v4, edge.v5);
            terrain.AddTriangleColor(color);
        }

        /// <summary>
        /// 三角化边界矩形
        /// </summary>
        /// <param name="e1"></param>
        /// <param name="c1"></param>
        /// <param name="e2"></param>
        /// <param name="c2"></param>
        void TriangulateEdgeStrip(EdgeVertices e1, Color c1, EdgeVertices e2, Color c2)
        {
            terrain.AddQuad(e1.v1, e1.v2, e2.v1, e2.v2);
            terrain.AddQuadColor(c1, c2);
            terrain.AddQuad(e1.v2, e1.v3, e2.v2, e2.v3);
            terrain.AddQuadColor(c1, c2);
            terrain.AddQuad(e1.v3, e1.v4, e2.v3, e2.v4);
            terrain.AddQuadColor(c1, c2);
            terrain.AddQuad(e1.v4, e1.v5, e2.v4, e2.v5);
            terrain.AddQuadColor(c1, c2);
        }

        /// <summary>
        /// 三角化阶梯边界
        /// </summary>
        /// <param name="beginLeft"></param>
        /// <param name="beginRight"></param>
        /// <param name="beginCell"></param>
        /// <param name="endLeft"></param>
        /// <param name="endRight"></param>
        /// <param name="endCell"></param>
        void TriangulateEdgeTerraces(EdgeVertices begin, HexCell beginCell,
                                     EdgeVertices end, HexCell endCell)
        {
            EdgeVertices e2 = EdgeVertices.TerraceLerp(begin, end, 1);
            Color c2 = HexMetrics.TerraceLerp(beginCell.Color, endCell.Color, 1);

            // 实现第一层阶梯，进行阶梯细分
            TriangulateEdgeStrip(begin, beginCell.Color, e2, c2);

            for (int i = 2; i < HexMetrics.terraceSteps; i++)
            {
                EdgeVertices e1 = e2;
                Color c1 = c2;

                e2 = EdgeVertices.TerraceLerp(begin, end, i);
                c2 = HexMetrics.TerraceLerp(beginCell.Color, endCell.Color, i);

                TriangulateEdgeStrip(e1, c1, e2, c2);
            }

            TriangulateEdgeStrip(e2, c2, end, endCell.Color);
        }

        /// <summary>
        /// 计算三个六边形相交的三角形
        /// </summary>
        /// <param name="bottom"></param>
        /// <param name="bottomCell"></param>
        /// <param name="left"></param>
        /// <param name="leftCell"></param>
        /// <param name="right"></param>
        /// <param name="rightCell"></param>
        void TriangulateCorner(Vector3 bottom, HexCell bottomCell,
                               Vector3 left, HexCell leftCell,
                               Vector3 right, HexCell rightCell)
        {
            HexEdgeType leftEdgeType = bottomCell.GetEdgeType(leftCell);
            HexEdgeType rightEdgeType = bottomCell.GetEdgeType(rightCell);

            if (leftEdgeType == HexEdgeType.Slope)
            {
                if (rightEdgeType == HexEdgeType.Slope)
                {
                    TriangulateCornerTerraces(
                        bottom, bottomCell, left, leftCell, right, rightCell
                    );
                }
                else if (rightEdgeType == HexEdgeType.Flat)
                {
                    TriangulateCornerTerraces(
                        left, leftCell, right, rightCell, bottom, bottomCell
                    );
                }
                else
                {
                    TriangulateCornerTerracesCliff(bottom, bottomCell, left, leftCell, right, rightCell);
                }
            }
            else if (rightEdgeType == HexEdgeType.Slope)
            {
                if (leftEdgeType == HexEdgeType.Flat)
                {
                    TriangulateCornerTerraces(
                        right, rightCell, bottom, bottomCell, left, leftCell
                    );
                }
                else
                {
                    TriangulateCornerCliffTerraces(
                        bottom, bottomCell, left, leftCell, right, rightCell
                    );
                }
            }

            // 左 陡坡 上 斜坡  右陡坡
            else if (leftCell.GetEdgeType(rightCell) == HexEdgeType.Slope)
            {
                if (leftCell.Elevation < rightCell.Elevation)
                {
                    TriangulateCornerCliffTerraces(right, rightCell, bottom, bottomCell, left, leftCell);
                }
                else
                {
                    TriangulateCornerTerracesCliff(left, leftCell, right, rightCell, bottom, bottomCell);
                }
            }
            else
            {
                terrain.AddTriangle(bottom, left, right);
                terrain.AddTriangleColor(bottomCell.Color, leftCell.Color, rightCell.Color);
            }
        }

        void TriangulateCornerTerraces(Vector3 begin, HexCell beginCell,
                                       Vector3 left, HexCell leftCell,
                                       Vector3 right, HexCell rightCell)
        {
            Vector3 v3 = HexMetrics.TerraceLerp(begin, left, 1);
            Vector3 v4 = HexMetrics.TerraceLerp(begin, right, 1);
            Color c3 = HexMetrics.TerraceLerp(beginCell.Color, leftCell.Color, 1);
            Color c4 = HexMetrics.TerraceLerp(beginCell.Color, rightCell.Color, 1);

            terrain.AddTriangle(begin, v3, v4);
            terrain.AddTriangleColor(beginCell.Color, c3, c4);

            for (int i = 2; i < HexMetrics.terraceSteps; i++)
            {
                Vector3 v1 = v3;
                Vector3 v2 = v4;
                Color c1 = c3;
                Color c2 = c4;
                v3 = HexMetrics.TerraceLerp(begin, left, i);
                v4 = HexMetrics.TerraceLerp(begin, right, i);
                c3 = HexMetrics.TerraceLerp(beginCell.Color, leftCell.Color, i);
                c4 = HexMetrics.TerraceLerp(beginCell.Color, rightCell.Color, i);
                terrain.AddQuad(v1, v2, v3, v4);
                terrain.AddQuadColor(c1, c2, c3, c4);
            }

            terrain.AddQuad(v3, v4, left, right);
            terrain.AddQuadColor(c3, c4, leftCell.Color, rightCell.Color);
        }

        /// <summary>
        /// 陡坡的边界三角形处理，陡坡在右侧
        /// </summary>
        /// <param name="begin"></param>
        /// <param name="beginCell"></param>
        /// <param name="left"></param>
        /// <param name="leftCell"></param>
        /// <param name="right"></param>
        /// <param name="rightCell"></param>
        void TriangulateCornerTerracesCliff(Vector3 begin, HexCell beginCell,
                                            Vector3 left, HexCell leftCell,
                                            Vector3 right, HexCell rightCell)
        {
            // 减去基准面高度来过去一个阶梯的比值
            float b = 1f / (rightCell.Elevation - beginCell.Elevation);
            if (b < 0)
            {
                b = -b;
            }
            Vector3 boundary = Vector3.Lerp(HexMetrics.Perturb(begin), HexMetrics.Perturb(right), b);
            Color boundaryColor = Color.Lerp(beginCell.Color, rightCell.Color, b);

            TriangulateBoundaryTriangle(begin, beginCell, left, leftCell, boundary, boundaryColor);

            if (leftCell.GetEdgeType(rightCell) == HexEdgeType.Slope)
            {
                TriangulateBoundaryTriangle(
                    left, leftCell, right, rightCell, boundary, boundaryColor
                );
            }
            else
            {
                terrain.AddTriangleUnperturbed(HexMetrics.Perturb(left), HexMetrics.Perturb(right), boundary);
                terrain.AddTriangleColor(leftCell.Color, rightCell.Color, boundaryColor);
            }
        }

        /// <summary>
        /// 陡坡的边界三角形处理，陡坡在左侧
        /// </summary>
        /// <param name="begin"></param>
        /// <param name="beginCell"></param>
        /// <param name="left"></param>
        /// <param name="leftCell"></param>
        /// <param name="right"></param>
        /// <param name="rightCell"></param>
        void TriangulateCornerCliffTerraces(Vector3 begin, HexCell beginCell,
                                            Vector3 left, HexCell leftCell,
                                            Vector3 right, HexCell rightCell)
        {
            float b = 1f / (leftCell.Elevation - beginCell.Elevation);
            if (b < 0)
            {
                b = -b;
            }
            Vector3 boundary = Vector3.Lerp(HexMetrics.Perturb(begin), HexMetrics.Perturb(left), b);
            Color boundaryColor = Color.Lerp(beginCell.Color, leftCell.Color, b);

            TriangulateBoundaryTriangle(
                right, rightCell, begin, beginCell, boundary, boundaryColor
            );

            if (leftCell.GetEdgeType(rightCell) == HexEdgeType.Slope)
            {
                TriangulateBoundaryTriangle(
                    left, leftCell, right, rightCell, boundary, boundaryColor
                );
            }
            else
            {
                terrain.AddTriangleUnperturbed(HexMetrics.Perturb(left), HexMetrics.Perturb(right), boundary);
                terrain.AddTriangleColor(leftCell.Color, rightCell.Color, boundaryColor);
            }
        }


        void TriangulateBoundaryTriangle(Vector3 begin, HexCell beginCell,
                                         Vector3 left, HexCell leftCell,
                                         Vector3 boundary, Color boundaryColor)
        {
            Vector3 v2 = HexMetrics.Perturb(HexMetrics.TerraceLerp(begin, left, 1));
            Color c2 = HexMetrics.TerraceLerp(beginCell.Color, leftCell.Color, 1);

            // 此处确定是否添加顶点扰动
            terrain.AddTriangleUnperturbed(HexMetrics.Perturb(begin), v2, boundary);
            terrain.AddTriangleColor(beginCell.Color, leftCell.Color, boundaryColor);

            for (int i = 2; i < HexMetrics.terraceSteps; i++)
            {
                Vector3 v1 = v2;
                Color c1 = c2;
                v2 = HexMetrics.Perturb(HexMetrics.TerraceLerp(begin, left, i));
                c2 = HexMetrics.TerraceLerp(beginCell.Color, leftCell.Color, i);
                // 此处确定是否添加顶点扰动
                terrain.AddTriangleUnperturbed(v1, v2, boundary);
                terrain.AddTriangleColor(c1, c2, boundaryColor);
            }
            // 此处确定是否添加顶点扰动
            terrain.AddTriangleUnperturbed(v2, HexMetrics.Perturb(left), boundary);
            terrain.AddTriangleColor(c2, leftCell.Color, boundaryColor);
        }

    }
}