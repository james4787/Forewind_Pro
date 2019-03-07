using UnityEngine;

public class HexCell : MonoBehaviour
{

    public HexCoordinates coordinates;
    // 获取六边形单体所属UILabel的引用
    public RectTransform uiRect;
    // 获取六边形单体所属区块的引用
    public HexGridChunk chunk;

    public Color Color
    {
        get
        {
            return color;
        }
        set
        {
            if (color == value)
            {
                return;
            }
            color = value;
            Refresh();
        }
    }

    public int Elevation
    {
        get
        {
            return elevation;
        }
        set
        {   
            // 如果高度相等则不更新网格数据
            if (elevation == value)
            {
                return;
            }
            // 设置六边形高度
            elevation = value;
            Vector3 position = transform.localPosition;
            position.y = value * HexMetrics.elevationStep;
            position.y +=
                (HexMetrics.SampleNoise(position).y * 2f - 1f) *
                HexMetrics.elevationPerturbStrength;
            transform.localPosition = position;

            Vector3 uiPosition = uiRect.localPosition;
            // UI高度设定，因为UI屏幕绕X轴旋转90度，所以高度沿Z轴负值增长
            uiPosition.z = -position.y;
            uiRect.localPosition = uiPosition;

            if (
                hasOutgoingRiver &&
                elevation < GetNeighbor(outgoingRiver).elevation
            )
            {
                RemoveOutgoingRiver();
            }
            if (
                hasIncomingRiver &&
                elevation > GetNeighbor(incomingRiver).elevation
            )
            {
                RemoveIncomingRiver();
            }
            // 区域刷新以更新区块网格
            Refresh();
        }
    }

    public bool HasIncomingRiver
    {
        get
        {
            return hasIncomingRiver;
        }
    }

    public bool HasOutgoingRiver
    {
        get
        {
            return hasOutgoingRiver;
        }
    }

    public bool HasRiver
    {
        get
        {
            return hasIncomingRiver || hasOutgoingRiver;
        }
    }

    public bool HasRiverBeginOrEnd
    {
        get
        {
            return hasIncomingRiver != hasOutgoingRiver;
        }
    }

    public HexDirection IncomingRiver
    {
        get
        {
            return incomingRiver;
        }
    }

    public HexDirection OutgoingRiver
    {
        get
        {
            return outgoingRiver;
        }
    }

    public Vector3 Position
    {
        get
        {
            return transform.localPosition;
        }
    }

    /// <summary>
    /// 获取河面高度
    /// </summary>
    public float RiverSurfaceY
    {
        get
        {
            return
                (elevation + HexMetrics.riverSurfaceElevationOffset) *
                HexMetrics.elevationStep;
        }
    }

    /// <summary>
    /// 获取河床高度
    /// </summary>
    public float StreamBedY
    {
        get
        {
            return
                (elevation + HexMetrics.streamBedElevationOffset) *
                HexMetrics.elevationStep;
        }
    }

    Color color;

    int elevation = int.MinValue;

    // 河流逻辑判断使用
    bool hasIncomingRiver, hasOutgoingRiver;
    HexDirection incomingRiver, outgoingRiver;

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

    public bool HasRiverThroughEdge(HexDirection direction)
    {
        return
            hasIncomingRiver && incomingRiver == direction ||
            hasOutgoingRiver && outgoingRiver == direction;
    }

    /// <summary>
    /// 移除流入河流
    /// </summary>
    public void RemoveIncomingRiver()
    {
        if (!hasIncomingRiver)
        {
            return;
        }
        hasIncomingRiver = false;
        RefreshSelfOnly();

        HexCell neighbor = GetNeighbor(incomingRiver);
        neighbor.hasOutgoingRiver = false;
        neighbor.RefreshSelfOnly();
    }

    /// <summary>
    /// 移除流出河流
    /// </summary>
    public void RemoveOutgoingRiver()
    {
        if (!hasOutgoingRiver)
        {
            return;
        }
        hasOutgoingRiver = false;
        RefreshSelfOnly();

        HexCell neighbor = GetNeighbor(outgoingRiver);
        neighbor.hasIncomingRiver = false;
        neighbor.RefreshSelfOnly();
    }

    /// <summary>
    /// 移除全部河流
    /// </summary>
    public void RemoveRiver()
    {
        RemoveOutgoingRiver();
        RemoveIncomingRiver();
    }

    /// <summary>
    /// 添加流出河流
    /// </summary>
    /// <param name="direction"></param>
    public void SetOutgoingRiver(HexDirection direction)
    {
        if (hasOutgoingRiver && outgoingRiver == direction)
        {
            return;
        }
        // 确保流出方向的对象不为空，且高度差为正进行向下流动
        HexCell neighbor = GetNeighbor(direction);
        if (!neighbor || elevation < neighbor.elevation)
        {
            return;
        }
        // 移除之前的流出或与设定方向一致的流入河流
        RemoveOutgoingRiver();
        if (hasIncomingRiver && incomingRiver == direction)
        {
            RemoveIncomingRiver();
        }
        // 设置流出河流
        hasOutgoingRiver = true;
        outgoingRiver = direction;
        RefreshSelfOnly();
        // 同时将相邻的设为流入河流
        neighbor.RemoveIncomingRiver();
        neighbor.hasIncomingRiver = true;
        neighbor.incomingRiver = direction.Opposite();
        neighbor.RefreshSelfOnly();
    }

    /// <summary>
    /// 区域刷新，更新区块网格
    /// </summary>
    void Refresh()
    {
        // 只有当获取到引用时才刷新
        if (chunk)
        {
            chunk.Refresh();
            // 相邻的不同区块也进行刷新
            for (int i = 0; i < neighbors.Length; i++)
            {
                HexCell neighbor = neighbors[i];
                if (neighbor != null && neighbor.chunk != chunk)
                {
                    neighbor.chunk.Refresh();
                }
            }
        }
    }

    /// <summary>
    /// 只刷新自身区块，用于移除河流
    /// </summary>
    void RefreshSelfOnly()
    {
        chunk.Refresh();
    }
}