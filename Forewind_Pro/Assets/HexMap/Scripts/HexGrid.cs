﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Forewind
{
    public class HexGrid : MonoBehaviour
    {
        // 横向晶胞数
        public int width = 6;
        // 纵向晶胞数
        public int height = 6;

        public Color defaultColor = Color.white;

        public HexCell cellPrefab;

        HexCell[] cells;
        public Text cellLabelPrefab;

        Canvas gridCanvas;
        HexMesh hexMesh;

        void Awake()
        {
            gridCanvas = GetComponentInChildren<Canvas>();
            hexMesh = GetComponentInChildren<HexMesh>();

            cells = new HexCell[height * width];

            for (int z = 0, i = 0; z < height; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    CreateCell(x, z, i++);
                }
            }
        }

        void Start()
        {
            hexMesh.Triangulate(cells);
        }

        /// <summary>
        /// 获取相应位置的六边形晶格引用
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public HexCell GetCell(Vector3 position)
        {
            position = transform.InverseTransformPoint(position);
            HexCoordinates coordinates = HexCoordinates.FromPosition(position);
            // 此处对应于偏移X的斜向序列，所以要加z/2
            int index = coordinates.X + coordinates.Z * width + coordinates.Z / 2;

            return cells[index];
        }

        /// <summary>
        /// 生成单位六边形
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <param name="i"></param>
        void CreateCell(int x, int z, int i)
        {
            Vector3 position;
            position.x = (x + 0.5f * z - z / 2) * (2.0f * HexMetrics.innerRadius);
            position.y = 0f;
            position.z = z * (1.5f * HexMetrics.outerRadius);

            HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
            cell.transform.SetParent(transform, false);
            cell.transform.localPosition = position;
            cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
            cell.color = defaultColor;

            if (x > 0)
            {
                cell.SetNeighbor(HexDirection.W, cells[i - 1]);
            }
            if (z > 0)
            {
                // 奇数行填充
                if ((z & 1) == 0)
                {
                    cell.SetNeighbor(HexDirection.SE, cells[i - width]);
                    if (x > 0)
                    {
                        cell.SetNeighbor(HexDirection.SW, cells[i - width - 1]);
                    }
                }
                // 偶数行填充
                else
                {
                    cell.SetNeighbor(HexDirection.SW, cells[i - width]);
                    if (x < width - 1)
                    {
                        cell.SetNeighbor(HexDirection.SE, cells[i - width + 1]);
                    }
                }
            }

            Text label = Instantiate<Text>(cellLabelPrefab);
            label.rectTransform.SetParent(gridCanvas.transform, false);
            label.rectTransform.anchoredPosition = new Vector2(position.x, position.z);
            label.text = cell.coordinates.ToStringOnSeparateLines();

            // 赋予每个晶胞UI组件引用
            cell.uiRect = label.rectTransform;

        }


        /// <summary>
        /// 将格栅进行三角化
        /// </summary>
        public void Refresh()
        {
            hexMesh.Triangulate(cells);
        }
    }
}