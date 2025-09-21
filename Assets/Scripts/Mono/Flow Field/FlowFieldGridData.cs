using UnityEngine;

[System.Serializable]
public class FlowFieldGridData
{
    public int width;
    public int height;
    public float nodeSize;
    public Vector2 originPosition;
    public GridNode[] nodes;
}
