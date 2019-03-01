using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Forewind
{
    public class HexGrid : MonoBehaviour
    {
        // 横向晶胞数  纵向晶胞数
        int cellCountX, cellCountZ;

        // 区块参数
        public int chunkCountX = 4, chunkCountZ = 3;

        public HexGridChunk chunkPrefab;
        HexGridChunk[] chunks;

        public Color defaultColor = Color.white;

        public HexCell cellPrefab;

        HexCell[] cells;
        public Text cellLabelPrefab;

        // 柏林噪声纹理
        public Texture2D noiseSource;

        void Awake()
        {
            HexMetrics.noiseSource = noiseSource;

            cellCountX = chunkCountX * HexMetrics.chunkSizeX;
            cellCountZ = chunkCountZ * HexMetrics.chunkSizeZ;

            CreateChunks();
            CreateCells();
        }

        void CreateChunks()
        {
            chunks = new HexGridChunk[chunkCountX * chunkCountZ];

            for (int z = 0, i = 0; z < chunkCountZ; z++)
            {
                for (int x = 0; x < chunkCountX; x++)
                {
                    HexGridChunk chunk = chunks[i++] = Instantiate(chunkPrefab);
                    chunk.transform.SetParent(transform);
                }
            }
        }

        void CreateCells()
        {
            cells = new HexCell[cellCountZ * cellCountX];

            for (int z = 0, i = 0; z < cellCountZ; z++)
            {
                for (int x = 0; x < cellCountX; x++)
                {
                    CreateCell(x, z, i++);
                }
            }
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
            int index = coordinates.X + coordinates.Z * cellCountX + coordinates.Z / 2;

            return cells[index];
        }

        /// <summary>
        /// 笔刷绘制六边形网格获取晶格引用
        /// </summary>
        /// <param name="coordinates"></param>
        /// <returns></returns>
        public HexCell GetCell(HexCoordinates coordinates)
        {
            int z = coordinates.Z;
            if (z < 0 || z >= cellCountZ)
            {
                return null;
            }
            int x = coordinates.X + z / 2;
            if (x < 0 || x >= cellCountX)
            {
                return null;
            }
            return cells[x + z * cellCountX];
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
            cell.transform.localPosition = position;
            cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
            cell.Color = defaultColor;

            if (x > 0)
            {
                cell.SetNeighbor(HexDirection.W, cells[i - 1]);
            }
            if (z > 0)
            {
                // 奇数行填充
                if ((z & 1) == 0)
                {
                    cell.SetNeighbor(HexDirection.SE, cells[i - cellCountX]);
                    if (x > 0)
                    {
                        cell.SetNeighbor(HexDirection.SW, cells[i - cellCountX - 1]);
                    }
                }
                // 偶数行填充
                else
                {
                    cell.SetNeighbor(HexDirection.SW, cells[i - cellCountX]);
                    if (x < cellCountX - 1)
                    {
                        cell.SetNeighbor(HexDirection.SE, cells[i - cellCountX + 1]);
                    }
                }
            }

            Text label = Instantiate<Text>(cellLabelPrefab);
            label.rectTransform.anchoredPosition = new Vector2(position.x, position.z);
            label.text = cell.coordinates.ToStringOnSeparateLines();
            // 赋予每个晶胞UI组件引用
            cell.uiRect = label.rectTransform;

            cell.Elevation = 0;

            AddCellToChunk(x, z, cell);
        }

        /// <summary>
        /// 将晶胞添加至相应的区块
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <param name="cell"></param>
        void AddCellToChunk(int x, int z, HexCell cell)
        {
            int chunkX = x / HexMetrics.chunkSizeX;
            int chunkZ = z / HexMetrics.chunkSizeZ;
            HexGridChunk chunk = chunks[chunkX + chunkZ * chunkCountX];

            int localX = x - chunkX * HexMetrics.chunkSizeX;
            int localZ = z - chunkZ * HexMetrics.chunkSizeZ;
            chunk.AddCell(localX + localZ * HexMetrics.chunkSizeX, cell);
        }

        /// <summary>
        /// 显示所有UI标识
        /// </summary>
        /// <param name="visible"></param>
        public void ShowUI(bool visible)
        {
            for (int i = 0; i < chunks.Length; i++)
            {
                chunks[i].ShowUI(visible);
            }
        }
    }
}