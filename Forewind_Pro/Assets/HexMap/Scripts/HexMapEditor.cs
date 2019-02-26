using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Forewind
{
	public class HexMapEditor : MonoBehaviour {

        public Color[] colors;

        public HexGrid hexGrid;

        private Color activeColor;
        int activeElevation = 0;

        /// <summary>
        /// 初始化起始颜色
        /// </summary>
        void Awake()
        {
            SelectColor(0);
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
            cell.color = activeColor;
            cell.Elevation = activeElevation;

            hexGrid.Refresh();
        }

        /// <summary>
        /// UI Toggle点击事件
        /// </summary>
        /// <param name="index"></param>
        public void SelectColor(int index)
        {
            activeColor = colors[index];
        }

        /// <summary>
        /// UI Slider滑动条注册事件
        /// </summary>
        /// <param name="elevation"></param>
        public void SetElevation(float elevation)
        {
            activeElevation = (int) elevation;
            Debug.Log("slider elevation:" + elevation);
        }
    }
}