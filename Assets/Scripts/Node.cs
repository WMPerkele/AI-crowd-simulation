using System.Collections;
using UnityEngine;

public class Node : IHeapItem<Node>
{
    public bool IsWalkable;
    public Vector3 WorldPos;
    //Distance from starting node
    public int GCost;
    //Distance from end node
    public int HCost;
    public int GridPosX, GridPosY;
    public Node NodeParent;
    private int m_index;

    public Node(bool isWalkable, Vector3 worldPos, int gridX, int gridY)
    {
        IsWalkable = isWalkable;
        WorldPos = worldPos;
        GridPosX = gridX;
        GridPosY = gridY;

    }

    public int FCost
    {
        get { return GCost + HCost; }
    }

    public int Index
    {
        get
        {
            return m_index;
        }
        set
        {
            m_index = value;
        }
    }

    public int CompareTo(Node nodeToCompare)
    {
        int _compare = FCost.CompareTo(nodeToCompare.FCost);
        if (_compare == 0)
            _compare = HCost.CompareTo(nodeToCompare.HCost);
        return -_compare;
    }
}
