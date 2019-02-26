﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Forewind
{
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
                transform.localPosition = position;

                // UI高度设定，因为UI屏幕绕X轴旋转90度，所以高度沿Z轴负值增长
                Vector3 uiPosition = uiRect.localPosition;
                uiPosition.z = elevation * -HexMetrics.elevationStep;
                uiRect.localPosition = uiPosition;
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