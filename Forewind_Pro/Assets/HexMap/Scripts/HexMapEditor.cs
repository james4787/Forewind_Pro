using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Forewind
{
    public class HexMapEditor : MonoBehaviour
    {

        public Color[] colors;

        public HexGrid hexGrid;

        private Color activeColor;
        int activeElevation = 0;
        bool applyColor = false;
        bool applyElevation = true;

        public Toggle blank;
        public Toggle elevation;
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
        }

        void HandleInput()
        {
            // 射线检测，世界坐标转换本地坐标
            Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(inputRay, out hit))
            {
                // 获取引用进行编辑
                EditCell(hexGrid.GetCell(hit.point));
            }
        }

        /// <summary>
        /// 编辑六边形晶胞参数
        /// </summary>
        /// <param name="cell"></param>
        void EditCell(HexCell cell)
        {
            if (applyColor)
            {
                cell.Color = activeColor;
            }
            if (applyElevation)
            {
                cell.Elevation = activeElevation;
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
            elevation.isOn = !applyColor;
            applyElevation = !applyColor;
        }

        /// <summary>
        /// UI Slider滑动条注册事件
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
            blank.isOn = toggle;
            applyElevation = toggle;
            applyColor = !toggle;
        }
    }
}