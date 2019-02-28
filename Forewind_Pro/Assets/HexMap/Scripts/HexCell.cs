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

        public Color color;

        private int elevation;

        public RectTransform uiRect;

        public int Elevation
        {
            get
            {
                return elevation;
            }
            set
            {
                Debug.Log(value);
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
    }
}