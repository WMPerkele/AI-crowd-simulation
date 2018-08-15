using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    private Grid m_grid;
    OverSeer m_overSeer;
    void Awake()
    {
        m_grid = GetComponent<Grid>();
        m_overSeer = GetComponent<OverSeer>();
    }

    public void StartFindPath(Vector3 startPos, Vector3 endPos)
    {
        StartCoroutine(FindPath(startPos, endPos));
    }

    IEnumerator FindPath(Vector3 startPos, Vector3 endPos)
    {
        Node _startNode = m_grid.GetNodeWithPosition(startPos);
        Node _endNode = m_grid.GetNodeWithPosition(endPos);

        Vector3[] _wayPoints = new Vector3[0];
        bool _pathSuccess = false;

        //If the startnode is not a walkable kind, find a close one that is
        if (!_startNode.IsWalkable)
            _startNode = m_grid.GetClosestWalkableNode(_startNode);
        if (!_endNode.IsWalkable)
            _endNode = m_grid.GetClosestWalkableNode(_endNode);

        if (_startNode.IsWalkable && _endNode.IsWalkable)
        {
            //Create heaps and hashsets for the nodes
            Heap<Node> _openNodes = new Heap<Node>(m_grid.MaxSize);
            HashSet<Node> _closedNodes = new HashSet<Node>();
            _openNodes.Add(_startNode);

            while (_openNodes.Count > 0)
            {
                Node _currentNode = _openNodes.RemoveFirst();
                _closedNodes.Add(_currentNode);

                //If we have found the endNode
                if (_currentNode == _endNode)
                {
                    _pathSuccess = true;
                    break;
                }
                //We now have a new node, get all the adjacent nodes that we should check in next loop and count their costs
                foreach (Node adjacent in m_grid.GetAdjacentNodes(_currentNode))
                {
                    //If the node isnt something we want to use, just ignore it
                    if (!adjacent.IsWalkable || _closedNodes.Contains(adjacent))
                        continue;

                    //Calculate the nodes distance from the starting node
                    int _newCostToAdjacent = _currentNode.GCost + GetCost(_currentNode, adjacent);
                    //If path to this adjacent node is shorter than our current path, add it to the list
                    if (_newCostToAdjacent < adjacent.GCost || !_openNodes.Contains(adjacent))
                    {
                        adjacent.GCost = _newCostToAdjacent;
                        adjacent.HCost = GetCost(adjacent, _endNode);
                        adjacent.NodeParent = _currentNode;

                        if(!_openNodes.Contains(adjacent))
                            _openNodes.Add(adjacent);
                    }
                }
            }
        }
        yield return null;
        if(_pathSuccess)
            _wayPoints = RetracePath(_startNode, _endNode);

        m_overSeer.FinishProcessingPath(_wayPoints, _pathSuccess);
    }

    //Go back down the path and reverse it so we can use it
    Vector3[] RetracePath(Node _startNode, Node _endNode)
    {
       List<Node> _path = new List<Node>();
        Node _currentNode = _endNode;

        while (_currentNode != _startNode)
        {
            _path.Add(_currentNode);
            _currentNode = _currentNode.NodeParent;
        }
        Vector3[] _wayPoints = SimplifyPath(_path);
        Array.Reverse(_wayPoints);

       return _wayPoints;
    }
    //Simplify the path by comparing directions of the lines between nodes
    Vector3[] SimplifyPath(List<Node> path)
    {
        List<Vector3> _wayPoints = new List<Vector3>();
        Vector2 _directionOld = Vector2.zero;

        for (int i = 1; i < path.Count; i++)
        {
            Vector2 _directionNew = new Vector2(path[i - 1].GridPosX - path[i].GridPosX, path[i - 1].GridPosY - path[i].GridPosY);
            if (_directionNew != _directionOld)
            {
                _wayPoints.Add(path[i].WorldPos);
            }

            _directionOld = _directionNew;
        }
        
        return _wayPoints.ToArray();
    }
    
    //Returns the HCost from Node A to Node B
    private int GetCost(Node nA, Node nB)
    {
        int _distX = Mathf.Abs(nA.GridPosX - nB.GridPosX);
        int _distY = Mathf.Abs(nA.GridPosY - nB.GridPosY);

        //The magic numbers are the side length and hypotenuse times 10
        if (_distX > _distY)
            return 14 * _distY + 10 * (_distX - _distY);

        return 14 * _distX + 10 * (_distY - _distX);
    }
}
