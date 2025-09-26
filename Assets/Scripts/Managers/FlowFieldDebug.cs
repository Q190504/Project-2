using UnityEngine;
using TMPro;
using Unity.Mathematics;

public enum FlowFieldDebugStatus
{
    None,
    Cost,
    BestCost,
    Vector,
}

public class FlowFieldDebug : MonoBehaviour
{
    public static FlowFieldDebug Instance { get; private set; }

    [SerializeField] private TMP_Text debugTextPrefab;
    [SerializeField] private Canvas debugCanvas;
    [SerializeField] private FlowFieldDebugStatus flowFieldDebugStatus;

    private TMP_Text[,] debugTexts;

    private FlowFieldGridData gridData; // switched to MonoBehaviour version

    private int width;
    private int height;
    private float cellSize;
    private Vector3 origin;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        gridData = FlowFieldManager.Instance.GetGridData();

        if (gridData == null)
        {
            Debug.LogWarning("FlowFieldManager singleton not found.");
            return;
        }

        width = gridData.width;
        height = gridData.height;
        cellSize = gridData.nodeSize;
        origin = gridData.originPosition;

        if (flowFieldDebugStatus == FlowFieldDebugStatus.Cost ||
            flowFieldDebugStatus == FlowFieldDebugStatus.BestCost)
        {
            InitializeDebugTexts();
        }
    }

    private void Update()
    {
        if (gridData == null) return;

        switch (flowFieldDebugStatus)
        {
            case FlowFieldDebugStatus.Cost:
                ShowCost();
                break;
            case FlowFieldDebugStatus.BestCost:
                ShowBestCost();
                break;
            case FlowFieldDebugStatus.Vector:
                ShowVector();
                break;
        }
    }

    private void InitializeDebugTexts()
    {
        debugTexts = new TMP_Text[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float3 nodeCenter = new float3(
                    origin.x + (x + 0.5f) * cellSize,
                    origin.y + (y + 0.5f) * cellSize,
                    0
                );
                TMP_Text text = Instantiate(debugTextPrefab, nodeCenter, Quaternion.identity, debugCanvas.transform);
                text.text = "";
                debugTexts[x, y] = text;
            }
        }
    }

    public void ShowCost()
    {
        if (gridData == null || gridData.nodes == null) return;

        foreach (var node in gridData.nodes)
        {
            debugTexts[node.x, node.y].text = node.cost.ToString();
        }
    }

    public void ShowBestCost()
    {
        if (gridData == null || gridData.nodes == null) return;

        foreach (var node in gridData.nodes)
        {
            debugTexts[node.x, node.y].text = node.bestCost.ToString();
        }
    }

    public void ShowVector()
    {
        if (gridData == null || gridData.nodes == null) return;

        foreach (var node in gridData.nodes)
        {
            int x = node.x;
            int y = node.y;

            float3 bottomLeft = new float3(origin.x + x * cellSize, origin.y + y * cellSize, 0);
            float3 topRight = bottomLeft + new float3(cellSize, cellSize, 0);
            float3 topLeft = new float3(bottomLeft.x, topRight.y, 0);
            float3 bottomRight = new float3(topRight.x, bottomLeft.y, 0);

            // Draw cell borders
            Debug.DrawLine(bottomLeft, bottomRight, Color.white);
            Debug.DrawLine(bottomRight, topRight, Color.white);
            Debug.DrawLine(topRight, topLeft, Color.white);
            Debug.DrawLine(topLeft, bottomLeft, Color.white);

            // Draw flow vector
            float3 center = bottomLeft + new float3(cellSize / 2f, cellSize / 2f, 0);
            float2 vec = node.vector;

            if (math.lengthsq(vec) > 0.0001f)
            {
                float3 dir = math.normalize(new float3(vec.x, vec.y, 0)) * (cellSize * 0.4f);
                float3 arrowEnd = center + dir;

                Debug.DrawLine(center, arrowEnd, Color.green);

                float3 arrowHead = math.normalize(dir) * (cellSize * 0.2f);

                float3 leftHead = arrowEnd + math.mul(quaternion.RotateZ(math.radians(135)), arrowHead);
                float3 rightHead = arrowEnd + math.mul(quaternion.RotateZ(math.radians(-135)), arrowHead);

                Debug.DrawLine(arrowEnd, leftHead, Color.green);
                Debug.DrawLine(arrowEnd, rightHead, Color.green);
            }
        }
    }
}
