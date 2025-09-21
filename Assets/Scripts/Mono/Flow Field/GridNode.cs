using UnityEngine;

[System.Serializable]
public struct GridNode
{
    public int index;
    public int x;
    public int y;
    public byte cost;
    public byte bestCost;
    public Vector2 vector;
}