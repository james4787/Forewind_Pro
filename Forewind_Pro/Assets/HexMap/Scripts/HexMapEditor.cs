using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Forewind
{
    public class HexMapEditor : MonoBehaviour
    {
        enum OptionalToggle
        {
            Ignore, Yes, No
        }

        OptionalToggle riverMode, roadMode;

        public Color[] colors;

        public HexGrid hexGrid;

        private Color activeColor;
        int activeElevation = 0;
        int activeWaterLevel;

        bool applyColor = false;
        bool applyElevation = true;
        bool applyWaterLevel = true;

        int brushSize;

        bool isDrag;
        HexDirection dragDirection;
        HexCell previousCell;
        /// <summary>
        /// 初始化起始颜色
        /// </summary>
        void Awake()
        {
            SelectColor(-1);
        }

        void Update()
        {
            if (
                Input.GetMouseButton(0) &&
                !EventSystem.current.IsPointerOverGameObject()
            )
            {
                HandleInput();
            }
            else
            {
                previousCell = null;
            }
        }

        void HandleInput()
        {
            // 射线检测，世界坐标转换本地坐标
            Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(inputRay, out hit))
            {
                HexCell currentCell = hexGrid.GetCell(hit.point);
                if (previousCell && previousCell != currentCell)
                {
                    ValidateDrag(currentCell);
                }
                else
                {
                    isDrag = false;
                }
                // 获取引用进行编辑
                EditCells(currentCell);
                previousCell = currentCell;
            }
            else
            {
                previousCell = null;
            }
        }

        /// <summary>
        /// 判断拖拽是否有效
        /// </summary>
        /// <param name="currentCell"></param>
        void ValidateDrag(HexCell currentCell)
        {
            for (dragDirection = HexDirection.NE; dragDirection <= HexDirection.NW; dragDirection++)
            {
                if (previousCell.GetNeighbor(dragDirection) == currentCell)
                {
                    isDrag = true;
                    return;
                }
            }
            isDrag = false;
        }

        /// <summary>
        /// 笔刷进行多个晶胞编辑
        /// </summary>
        /// <param name="center"></param>
        void EditCells(HexCell center)
        {
            int centerX = center.coordinates.X;
            int centerZ = center.coordinates.Z;

            for (int r = 0, z = centerZ - brushSize; z <= centerZ; z++, r++)
            {
                for (int x = centerX - r; x <= centerX + brushSize; x++)
                {
                    EditCell(hexGrid.GetCell(new HexCoordinates(x, z)));
                }
            }
            for (int r = 0, z = centerZ + brushSize; z > centerZ; z--, r++)
            {
                for (int x = centerX - brushSize; x <= centerX + r; x++)
                {
                    EditCell(hexGrid.GetCell(new HexCoordinates(x, z)));
                }
            }
        }

        /// <summary>
        /// 编辑六边形晶胞参数
        /// </summary>
        /// <param name="cell"></param>
        void EditCell(HexCell cell)
        {
            if (cell)
            {
                if (applyColor)
                {
                    cell.Color = activeColor;
                }
                if (applyElevation)
                {
                    cell.Elevation = activeElevation;
                }
                if (applyWaterLevel)
                {
                    cell.WaterLevel = activeWaterLevel;
                }
                if (riverMode == OptionalToggle.No)
                {
                    cell.RemoveRiver();
                }
                if (roadMode == OptionalToggle.No)
                {
                    cell.RemoveRoads();
                }
                if (isDrag)
                {
                    // 支持笔刷绘制河流，主要利用获取的拖拽方向
                    HexCell otherCell = cell.GetNeighbor(dragDirection.Opposite());
                    if (otherCell)
                    {
                        if (riverMode == OptionalToggle.Yes)
                        {
                            // 设置流出河流方向
                            otherCell.SetOutgoingRiver(dragDirection);
                        }
                        if (roadMode == OptionalToggle.Yes)
                        {
                            // 设置道路走向
                            otherCell.AddRoad(dragDirection);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// UI Toggle点击事件
        /// </summary>
        /// <param name="index"></param>
        public void SelectColor(int index)
        {
            applyColor = index >= 0;
            if (applyColor)
            {
                activeColor = colors[index];
            }
        }

        /// <summary>
        /// UI Slider设置高度注册事件
        /// </summary>
        /// <param name="elevation"></param>
        public void SetElevation(float elevation)
        {
            activeElevation = (int) elevation;
        }

        /// <summary>
        /// UI Toggle高度选项注册事件
        /// </summary>
        /// <param name="toggle"></param>
        public void SetApplyElevation(bool toggle)
        {
            applyElevation = toggle;
        }

        /// <summary>
        /// UI Slider 笔刷大小注册事件
        /// </summary>
        /// <param name="size"></param>
        public void SetBrushSize(float size)
        {
            brushSize = (int) size;
        }

        /// <summary>
        /// 显示所有UI标识
        /// </summary>
        /// <param name="visible"></param>
        public void ShowUI(bool visible)
        {
            hexGrid.ShowUI(visible);
        }

        /// <summary>
        /// 设置河流编辑模式 UI注册事件
        /// </summary>
        /// <param name="mode"></param>
        public void SetRiverMode(int mode)
        {
            riverMode = (OptionalToggle) mode;
        }

        /// <summary>
        /// 设置道路编辑模式 UI注册事件
        /// </summary>
        /// <param name="mode"></param>
        public void SetRoadMode(int mode)
        {
            roadMode = (OptionalToggle) mode;
        }

        /// <summary>
        /// UI 设置水面高度注册事件
        /// </summary>
        /// <param name="toggle"></param>
        public void SetApplyWaterLevel(bool toggle)
        {
            applyWaterLevel = toggle;
        }

        /// <summary>
        /// UI 水面高度数据OnValueChange事件
        /// </summary>
        /// <param name="level"></param>
        public void SetWaterLevel(float level)
        {
            activeWaterLevel = (int) level;
        }
    }
}