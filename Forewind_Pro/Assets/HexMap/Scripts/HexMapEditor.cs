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
        int activeElevation;

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
            Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(inputRay, out hit))
            {
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
            hexGrid.Refresh();
        }

        public void SelectColor(int index)
        {
            activeColor = colors[index];
        }
    }
}