using UnityEngine;

public static class HexMetrics
{
    // 外半径转内半径系数
    public const float outerToInner = 0.866025404f;
    // 内半径转外半径系数
    public const float innerToOuter = 1f / outerToInner;
    // 六边形外半径大小
    public const float outerRadius = 10f;
    // 六边形内半径大小
    public const float innerRadius = outerRadius * outerToInner;
    // 固定纯色因子
    public const float solidFactor = 0.8f;
    // 混合因子
    public const float blendFactor = 1f - solidFactor;
    // 单位高度改变大小
    public const float elevationStep = 3f;
    // 斜坡阶梯数量
    public const int terracesPerSlope = 2;
    // 阶梯数量
    public const int terraceSteps = terracesPerSlope * 2 + 1;
    // 单位阶梯的水平步长
    public const float horizontalTerraceStepSize = 1f / terraceSteps;
    // 单位阶梯的垂直步长
    public const float verticalTerraceStepSize = 1f / (terracesPerSlope + 1);
    // 噪声扰动强度，强度为一时，最大值为根号三
    public const float cellPerturbStrength = 4f;
    // 顶点高度扰动强度
    public const float elevationPerturbStrength = 1.5f;
    // 河床位置向下偏移高度
    public const float streamBedElevationOffset = -1.75f;
    // 水面高度偏移
    public const float waterElevationOffset = -0.5f;
    // 噪声取样范围
    public const float noiseScale = 0.003f;
    // 区块单位数
    public const int chunkSizeX = 5, chunkSizeZ = 5;
    // 边界水面因数
    public const float waterFactor = 0.6f;
    // 水面网格桥接因数
    public const float waterBlendFactor = 1f - waterFactor;
    /// <summary>
    /// 六边形各顶点的位置描述
    /// </summary>
    static Vector3[] corners = {
        new Vector3(0f, 0f, outerRadius),
        new Vector3(innerRadius, 0f, 0.5f * outerRadius),
        new Vector3(innerRadius, 0f, -0.5f * outerRadius),
        new Vector3(0f, 0f, -outerRadius),
        new Vector3(-innerRadius, 0f, -0.5f * outerRadius),
        new Vector3(-innerRadius, 0f, 0.5f * outerRadius),
        new Vector3(0f, 0f, outerRadius)
    };

    public static Vector3 GetFirstWaterCorner(HexDirection direction)
    {
        return corners[(int) direction] * waterFactor;
    }

    public static Vector3 GetSecondWaterCorner(HexDirection direction)
    {
        return corners[(int) direction + 1] * waterFactor;
    }

    public static Vector3 GetWaterBridge(HexDirection direction)
    {
        return (corners[(int) direction] + corners[(int) direction + 1]) *
            waterBlendFactor;
    }

    // 获取噪声引用
    public static Texture2D noiseSource;

    /// <summary>
    /// 获取样本噪声
    /// </summary>
    /// <param name="position">噪声采样点</param>
    /// <returns>噪声数据</returns>
    public static Vector4 SampleNoise(Vector3 position)
    {
        return noiseSource.GetPixelBilinear(
            position.x * noiseScale,
            position.z * noiseScale
        );
    }

    public static Vector3 GetFirstCorner(HexDirection direction)
    {
        return corners[(int) direction];
    }

    public static Vector3 GetSecondCorner(HexDirection direction)
    {
        return corners[(int) direction + 1];
    }

    /// <summary>
    /// 获取纯色内顶点
    /// </summary>
    /// <param name="direction">指定顶点方向</param>
    /// <returns>顶点位置向量</returns>
    public static Vector3 GetFirstSolidCorner(HexDirection direction)
    {
        return corners[(int) direction] * solidFactor;
    }

    /// <summary>
    /// 获取纯色内顶点
    /// </summary>
    /// <param name="direction">指定顶点方向</param>
    /// <returns>顶点位置向量</returns>
    public static Vector3 GetSecondSolidCorner(HexDirection direction)
    {
        return corners[(int) direction + 1] * solidFactor;
    }

    /// <summary>
    /// 获取纯色内边的中间点
    /// </summary>
    /// <param name="direction">指定顶点方向</param>
    /// <returns>中点位置向量</returns>
    public static Vector3 GetSolidEdgeMiddle(HexDirection direction)
    {
        return
            (corners[(int) direction] + corners[(int) direction + 1]) *
            (0.5f * solidFactor);
    }

    /// <summary>
    /// 计算缩小区域（矩形）的宽度向量
    /// </summary>
    /// <param name="direction">指定顶点方向</param>
    /// <returns></returns>
    public static Vector3 GetBridge(HexDirection direction)
    {
        return (corners[(int) direction] + corners[(int) direction + 1]) *
            blendFactor;
    }

    /// <summary>
    /// 阶梯插值
    /// </summary>
    /// <param name="a">起始坐标</param>
    /// <param name="b">终点坐标</param>
    /// <param name="step">插值数</param>
    /// <returns>插值坐标</returns>
    public static Vector3 TerraceLerp(Vector3 a, Vector3 b, int step)
    {
        float h = step * HexMetrics.horizontalTerraceStepSize;
        a.x += (b.x - a.x) * h;
        a.z += (b.z - a.z) * h;
        float v = ((step + 1) / 2) * HexMetrics.verticalTerraceStepSize;
        a.y += (b.y - a.y) * v;
        return a;
    }

    /// <summary>
    /// 阶梯的颜色插值
    /// </summary>
    /// <param name="a">起始坐标颜色</param>
    /// <param name="b">终点坐标颜色</param>
    /// <param name="step">插值数</param>
    /// <returns>插值颜色</returns>
    public static Color TerraceLerp(Color a, Color b, int step)
    {
        float h = step * HexMetrics.horizontalTerraceStepSize;
        return Color.Lerp(a, b, h);
    }

    /// <summary>
    /// 获取相应高度差的类型（平地 斜坡 陡坡）
    /// </summary>
    /// <param name="elevation1">起始高度</param>
    /// <param name="elevation2">对照高度</param>
    /// <returns>斜坡类型</returns>
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
    /// 利用噪声来扰动顶点，使顶点扰动范围为-1到1
    /// </summary>
    /// <param name="position">扰动顶点对象</param>
    /// <returns>添加噪声的顶点</returns>
    public static Vector3 Perturb(Vector3 position)
    {
        Vector4 sample = SampleNoise(position);

        position.x += (sample.x * 2f - 1f) * cellPerturbStrength;
        // 为克服单元不对称，不对纵坐标加偏移使中心平面扁平
        //position.y += (sample.y * 2f - 1f) * HexMetrics.cellPerturbStrength;
        position.z += (sample.z * 2f - 1f) * cellPerturbStrength;
        return position;
    }
}