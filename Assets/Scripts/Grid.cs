using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public bool DisplayGrid;
    public LayerMask ObstacleLayer;
    public Vector2 GridSize;
    public float NodeRadius;

    private Node[,] m_grid;
    private float nodeSize;
    private int gridSizeX, gridSizeY;


    public int MaxSize
    {
        get
        {
            return gridSizeX * gridSizeY;
        }
    }
    void Awake()
    {
        nodeSize = NodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(GridSize.x / nodeSize);
        gridSizeY = Mathf.RoundToInt(GridSize.y / nodeSize);
        GenerateGrid();
    }

    void GenerateGrid()
    {
        m_grid = new Node[gridSizeX, gridSizeY];
        //Get the bottom of the grid so we know from where to start bulding
        Vector3 _bottomLeft = transform.position -
            Vector3.right * GridSize.x / 2 -
            Vector3.forward * GridSize.y / 2;

        //Get the node size before iterating to not make a new size each iteration
        Vector3 _nodeSize = new Vector3(NodeRadius, 1, NodeRadius);

        for (int i = 0; i < gridSizeX; i++)
        {
            for (int j = 0; j < gridSizeY; j++)
            {
                //Get the point where the node will be
                Vector3 _worldPoint = _bottomLeft +
                    Vector3.right * (i * nodeSize + NodeRadius) +
                    Vector3.forward * (j * nodeSize + NodeRadius);

                //Check if the area has an obstacle on it
                bool _isWalkable = !Physics.CheckBox(_worldPoint, _nodeSize, Quaternion.identity, ObstacleLayer);

                //Create a new node
                m_grid[i, j] = new Node(_isWalkable, _worldPoint, i, j);
            }
        }
    }

    public List<Node> GetAdjacentNodes(Node node)
    {
        List<Node> _adjacents = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int _checkX = node.GridPosX + x;
                int _checkY = node.GridPosY + y;

                if(_checkX >= 0 && _checkX < gridSizeX && _checkY >= 0 && _checkY < gridSizeY )
                {
                    _adjacents.Add(m_grid[_checkX, _checkY]);
                }
            }
        }
        return _adjacents;

    }


    //Returns the node that in these coordinates
    public Node GetNodeWithPosition(Vector3 position)
    {
        //Check that we are not getting a node out of the bounds of grid
        if (position.x > transform.position.x + GridSize.x || position.z > transform.position.z + GridSize.y
            || position.x < transform.position.x - GridSize.x || position.z < transform.position.z - GridSize.y)
        {
            Debug.Log("Position of: " + position + " Is out of grid bounds");
        }

        float _percentX = (position.x + GridSize.x / 2) / GridSize.x;
        float _percentY = (position.z + GridSize.y / 2) / GridSize.y;

        _percentX = Mathf.Clamp01(_percentX);
        _percentY = Mathf.Clamp01(_percentY);

        int _x = Mathf.RoundToInt((gridSizeX - 1) * _percentX);
        int _y = Mathf.RoundToInt((gridSizeY - 1) * _percentY);



        return m_grid[_x, _y];
    }
    //Get the closest node that is walkable
    public Node GetClosestWalkableNode(Node node)
    {
        Node _returnNode = null;
        for (int x = 0; x < gridSizeX; x++)
            for (int y = 0; y < gridSizeY; y++)
            {
                Node _testNode = m_grid[x, y];
                if (_testNode.IsWalkable)
                {
                    if (Mathf.Abs(node.GridPosX - _testNode.GridPosX) + Mathf.Abs(node.GridPosY - _testNode.GridPosY) <
                        Mathf.Abs(node.GridPosX - _returnNode.GridPosX) + Mathf.Abs(node.GridPosY - _returnNode.GridPosY))
                        _returnNode = _testNode;
                }
            }
        return _returnNode;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(GridSize.x, 1, GridSize.y));

        if (m_grid != null && DisplayGrid)
        {
            foreach (Node node in m_grid)
            {
                Gizmos.color = (node.IsWalkable) ? Color.white : Color.red;
                Gizmos.DrawCube(node.WorldPos, Vector3.one * nodeSize);
            }
        }
    }



}
