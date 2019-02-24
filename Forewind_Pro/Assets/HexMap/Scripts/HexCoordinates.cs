using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Forewind
{
    [System.Serializable]
    public class HexCoordinates
    {

        [SerializeField]
        private int x, z;

        public int X
        {
            get
            {
                return x;
            }
        }

        public int Y
        {
            get
            {
                return -X - Z;
            }
        }

        public int Z
        {
            get
            {
                return z;
            }
        }

        public HexCoordinates(int x, int z)
        {
            this.x = x;
            this.z = z;
        }

        /// <summary>
        /// 修正X坐标让它们沿直线斜向排开
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static HexCoordinates FromOffsetCoordinates(int x, int z)
        {
            return new HexCoordinates(x - z / 2, z);
        }

        public static HexCoordinates FromPosition(Vector3 position)
        {
            float x = position.x / (HexMetrics.innerRadius * 2f);
            float y = -x;

            // z轴移动一个，x，y反向偏移半个
            float offset = position.z / (HexMetrics.outerRadius * 3f / 2f);
            x -= offset / 2f;
            y -= offset / 2f;

            int iX = Mathf.RoundToInt(x);
            int iY = Mathf.RoundToInt(y);
            int iZ = Mathf.RoundToInt(-x - y);

            // 如果三坐标和不为零则打印错误
            if (iX + iY + iZ != 0)
            {
                float dX = Mathf.Abs(x - iX);
                float dY = Mathf.Abs(y - iY);
                float dZ = Mathf.Abs(-x - y - iZ);

                if (dX > dY && dX > dZ)
                {
                    iX = -iY - iZ;
                }
                else if (dZ > dY)
                {
                    iZ = -iX - iY;
                }
            }
           

            if (iX + iY + iZ != 0)
            {
                Debug.LogWarning("rounding error!");
            }

                return new HexCoordinates(iX, iZ);
        }

        /// <summary>
        /// 重写Tostring方法
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "(" +
                X.ToString() + ", " + Y.ToString() + ", " + Z.ToString() + ")";
        }


        /// <summary>
        /// toString的不同行输出方法
        /// </summary>
        /// <returns></returns>
        public string ToStringOnSeparateLines()
        {
            return X.ToString() + "\n" + Y.ToString() + "\n" + Z.ToString();
        }
    }
}