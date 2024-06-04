using System;
using UnityEngine;
using System.Collections.Generic;

public class PathfindingSurface : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private int _width;
    [SerializeField] private int _height;
    [SerializeField] private float _cellSize;
    [SerializeField] private int _defaultNodePriority;
    [Space]
    [SerializeField] private List<SearchPoint> _searchPoints;

    private Grid<PathfindingNode> _grid;

    public float CellSize => _cellSize;
    public Grid<PathfindingNode> Grid => _grid;

    private void Awake()
    {
        BakeSurface();
    }

    private void BakeSurface()
    {
        Func<int, int, PathfindingNode> createPathNode = (int x, int y) => new PathfindingNode(x, y, true, _defaultNodePriority); // Function to create pathNodes
        _grid = new Grid<PathfindingNode>(_width, _height, _cellSize, transform.position, createPathNode); // Instantiates the grid

        PathfindingArea[] pathfindingAreas = FindObjectsOfType<PathfindingArea>();
        foreach (PathfindingArea area in pathfindingAreas)
        {
            if (!area.AffectsSurface(this)) continue;

            area.ConfigureAffectedPathNodes();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (_grid == null)
        {
            Gizmos.color = Color.black;
        }
        else
        {
            Gizmos.color = Color.white;
        }

        for (int x = 0; x <= _width; x++)
        {
            Vector3 startPosition = transform.position + new Vector3(x, 0) * _cellSize;
            Vector3 endPosition = transform.position + new Vector3(x, _height) * _cellSize;

            Gizmos.DrawLine(startPosition, endPosition);
        }

        // Draw horizontal grid lines
        for (int y = 0; y <= _height; y++)
        {
            Vector3 startPosition = transform.position + new Vector3(0, y) * _cellSize;
            Vector3 endPosition = transform.position + new Vector3(_width, y) * _cellSize;

            Gizmos.DrawLine(startPosition, endPosition);
        }

        if (_grid == null) return;

        Gizmos.color = Color.blue;

        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                PathfindingNode pathNode = _grid.GetGridObject(x, y);
                if (!pathNode.IsWalkable)
                {
                    Vector3 center = _grid.GetWorldPosition(x, y);
                    Vector3 size = Vector3.one * _cellSize;

                    Gizmos.DrawCube(center, size);
                }
            }
        }
    }

    public SearchPoint GetClosestSearchPoint(Vector3 originPosition)
    {
        int closestIndex = 0;
        float closestSqrDistance = 1000f;

        for (int i = 0; i < _searchPoints.Count; i++)
        {
            float comparedSqrDistance = (_searchPoints[i].transform.position - originPosition).sqrMagnitude;

            if (comparedSqrDistance < closestSqrDistance)
            {
                closestSqrDistance = comparedSqrDistance;
                closestIndex = i;
            }
        }

        return _searchPoints[closestIndex];
    }
}
