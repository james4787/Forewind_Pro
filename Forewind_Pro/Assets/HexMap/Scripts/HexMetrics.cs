using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Forewind
{
    public enum HexEdgeType
    {
        Flat, Slope, Cliff
    }

    public static class HexMetrics
    {

        public const float outerRadius = 10.0f;

        public const float innerRadius = outerRadius * 0.866025404f;

        public const float solidFactor = 0.8f;

        public const float blendFactor = 1f - solidFactor;

        public const float elevationStep = 3f;

        public const int terracesPerSlope = 2;
        // 阶梯数量
        public const int terraceSteps = terracesPerSlope * 2 + 1;
        // 单位阶梯的水平步长
        public const float horizontalTerraceStepSize = 1f / terraceSteps;
        // 单位阶梯的垂直步长
        public const float verticalTerraceStepSize = 1f / (terracesPerSlope + 1);
        // 柏林噪声纹理
        public static Texture2D noiseSource;
        // 噪声扰动强度，强度为一时，最大值为根号三
        public const float cellPerturbStrength = 4f;
        // 噪声取样范围
        public const float noiseScale = 0.003f;
        // 垂直方向扰动强度
        public const float elevationPerturbStrength = 1.5f;
        // 区块单位数
        public const int chunkSizeX = 5, chunkSizeZ = 5;

        static Vector3[] corners = {
            new Vector3(0f, 0f, outerRadius),
            new Vector3(innerRadius, 0f, 0.5f * outerRadius),
            new Vector3(innerRadius, 0f, -0.5f * outerRadius),
            new Vector3(0f, 0f, -outerRadius),
            new Vector3(-innerRadius, 0f, -0.5f * outerRadius),
            new Vector3(-innerRadius, 0f, 0.5f * outerRadius),
            new Vector3(0f, 0f, outerRadius)
        };

        public static Vector3 GetFirstCorner(HexDirection direction)
        {
            return corners[(int) direction];
        }

        public static Vector3 GetSecondCorner(HexDirection direction)
        {
            return corners[(int) direction + 1];
        }

        public static Vector3 GetFirstSolidCorner(HexDirection direction)
        {
            return corners[(int) direction] * solidFactor;
        }

        public static Vector3 GetSecondSolidCorner(HexDirection direction)
        {
            return corners[(int) direction + 1] * solidFactor;
        }

        /// <summary>
        /// 计算缩小区域的宽度向量
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static Vector3 GetBridge(HexDirection direction)
        {
            return (corners[(int) direction] + corners[(int) direction + 1]) * blendFactor;
        }

        /// <summary>
        /// 阶梯插值
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public static Vector3 TerraceLerp(Vector3 a, Vector3 b, int step)
        {
            float h = step * HexMetrics.horizontalTerraceStepSize;
            a.x += (b.x - a.x) * h;
            a.z += (b.z - a.z) * h;

            // 奇数步数调整高度
            float v = ((step + 1) / 2) * HexMetrics.verticalTerraceStepSize;
            a.y += (b.y - a.y) * v;

            return a;
        }

        /// <summary>
        /// 阶梯的颜色插值
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public static Color TerraceLerp(Color a, Color b, int step)
        {
            float h = step * HexMetrics.horizontalTerraceStepSize;
            return Color.Lerp(a, b, h);
        }

        /// <summary>
        /// 获取高度差类型 扁平 斜坡 陡坡
        /// </summary>
        /// <param name="elevation1"></param>
        /// <param name="elevation2"></param>
        /// <returns>返回HexEdgeType类型</returns>
        public static HexEdgeType GetEdgeType(int elevation1, int elevation2)
        {
            if (elevation1 == elevation2)
            {
                return HexEdgeType.Flat;
            }
            int delta = elevation2 - elevation1;
            if (delta == 1 || delta == -1)
            {
                return HexEdgeType.Slope;
            }
            return HexEdgeType.Cliff;
        }

        /// <summary>
        /// 获取样本噪声
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public static Vector4 SampleNoise(Vector3 position)
        {
            return noiseSource.GetPixelBilinear(
                position.x * noiseScale,
                position.z * noiseScale
            );
        }
    }
}