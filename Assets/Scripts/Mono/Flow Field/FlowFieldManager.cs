using UnityEngine;
using System.Collections.Generic;

public class FlowFieldManager : MonoBehaviour
{
    #region Singleton
    public static FlowFieldManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    #endregion

    [Header("Grid Settings")]
    [SerializeField] int width;
    [SerializeField] int height;
    [SerializeField] float nodeSize = 1f;
    [SerializeField] Vector2 originPosition = Vector2.zero;

    [Header("References")]
    [SerializeField] Transform player; // Destination target
    FlowFieldGridData grid;

    void Start()
    {
        InitializeGrid();
        ComputeFlowField();
    }

    /// <summary> Full flow field computation pipeline </summary>
    public void ComputeFlowField()
    {
        ComputeIntegrationField();
        ComputeFlowDirections();
        Debug.Log("Flow field computed!");
    }

    #region Initialization
    void InitializeGrid()
    {
        grid = new FlowFieldGridData
        {
            width = width,
            height = height,
            nodeSize = nodeSize,
            originPosition = originPosition,
            nodes = new GridNode[width * height]
        };

        float nodeRadius = nodeSize / 2f;

        for (int i = 0; i < grid.nodes.Length; i++)
        {
            int x = i % width;
            int y = i / width;

            Vector2 nodeWorldPos = new Vector2(nodeSize * x + nodeRadius, nodeSize * y + nodeRadius);
            byte nodeCost = CostFieldHelper.instance.EvaluateCost(nodeWorldPos, nodeSize);

            grid.nodes[i] = new GridNode
            {
                index = i,
                x = x,
                y = y,
                cost = nodeCost,
                bestCost = byte.MaxValue,
                vector = Vector2.zero
            };
        }
    }
    #endregion

    #region Integration Field
    void ComputeIntegrationField()
    {
        // Reset
        for (int i = 0; i < grid.nodes.Length; i++)
        {
            grid.nodes[i].bestCost = byte.MaxValue;
            grid.nodes[i].vector = Vector2.zero;
        }

        // Destination node
        Vector2 playerPos = player.position;
        int destinationIndex = GetNodeIndexFromPosition(playerPos);
        grid.nodes[destinationIndex].bestCost = 0;

        Queue<int> openList = new Queue<int>();
        openList.Enqueue(destinationIndex);

        while (openList.Count > 0)
        {
            int current = openList.Dequeue();
            int x = grid.nodes[current].x;
            int y = grid.nodes[current].y;
            byte currentCost = grid.nodes[current].bestCost;

            Vector2Int[] neighbors = {
                new Vector2Int(-1,0), new Vector2Int(1, 0),
                new Vector2Int(0,-1), new Vector2Int(0, 1)
            };

            foreach (var offset in neighbors)
            {
                int nx = x + offset.x;
                int ny = y + offset.y;

                if (nx < 0 || nx >= grid.width || ny < 0 || ny >= grid.height)
                    continue;

                int neighborIndex = nx + ny * grid.width;
                byte cost = grid.nodes[neighborIndex].cost;
                if (cost == byte.MaxValue) continue;

                byte newCost = (byte)(currentCost + cost);
                if (newCost < grid.nodes[neighborIndex].bestCost)
                {
                    grid.nodes[neighborIndex].bestCost = newCost;
                    openList.Enqueue(neighborIndex);
                }
            }
        }
    }
    #endregion

    #region Flow Direction
    void ComputeFlowDirections()
    {
        Vector2Int[] neighborOffsets = {
            new Vector2Int(-1, 0), new Vector2Int(1, 0),
            new Vector2Int(0, -1), new Vector2Int(0, 1),
            new Vector2Int(-1, -1), new Vector2Int(1, -1),
            new Vector2Int(-1, 1), new Vector2Int(1, 1)
        };

        for (int i = 0; i < grid.nodes.Length; i++)
        {
            int x = grid.nodes[i].x;
            int y = grid.nodes[i].y;
            int lowestCost = grid.nodes[i].bestCost;
            Vector2 bestDir = Vector2.zero;

            foreach (var offset in neighborOffsets)
            {
                int nx = x + offset.x;
                int ny = y + offset.y;
                if (nx < 0 || nx >= grid.width || ny < 0 || ny >= grid.height)
                    continue;

                int neighborIndex = nx + ny * grid.width;
                int neighborCost = grid.nodes[neighborIndex].bestCost;

                if (neighborCost < lowestCost)
                {
                    lowestCost = neighborCost;
                    bestDir = new Vector2(offset.x, offset.y).normalized;
                }
            }

            grid.nodes[i].vector = bestDir;
        }
    }
    #endregion

    #region Utility
    int GetNodeIndexFromPosition(Vector2 pos)
    {
        int x = Mathf.Clamp((int)((pos.x - grid.originPosition.x) / grid.nodeSize), 0, grid.width - 1);
        int y = Mathf.Clamp((int)((pos.y - grid.originPosition.y) / grid.nodeSize), 0, grid.height - 1);
        return y * grid.width + x;
    }

    public Vector2 GetDirectionFromIndex(int index)
    {
        if (index >= 0 && index < grid.nodes.Length)
            return grid.nodes[index].vector;
        else return Vector2.zero;
    }

    public FlowFieldGridData GetGridData()
    {
        return grid;
    }

    public float GetNodeSize()
    {
        return nodeSize;
    }

    public int GetMapWidth()
    {
        return width;
    }

    #endregion
}
