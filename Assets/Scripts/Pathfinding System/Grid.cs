using System;
using UnityEditor;
using UnityEngine;

public class Grid<TGridObject>
{
    private int _width;
    private int _height;
    private float _cellSize;
    private Vector3 _originPosition;
    private TGridObject[,] _gridArray;

    public int Width => _width;
    public int Height => _height;
    public float CellSize => _cellSize;

    public Grid(int width, int height, float cellSize, Vector3 originPosition, Func<int, int, TGridObject> createGridObject)
    {
        _width = width;
        _height = height;
        _cellSize = cellSize;
        _originPosition = originPosition;

        _gridArray = new TGridObject[width, height];

        // Fill grid array with grid objects
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                _gridArray[x, y] = createGridObject(x, y);
            }
        }
    }

    public Vector3 GetWorldPosition(int x, int y) // Return world position of GridObject
    {
        return (new Vector3(x, y) * _cellSize + new Vector3(1, 1) * _cellSize / 2f) + _originPosition;
    }

    public void GetGridPosition(Vector3 worldPosition, out int x, out int y) // Return world position transfered in local position in grid
    {
        Vector3 gridPosition = worldPosition - _originPosition;

        x = Mathf.FloorToInt(gridPosition.x);
        y = Mathf.FloorToInt(gridPosition.y);
    }

    public TGridObject GetGridObject(int x, int y) // Return GridObject by it's coordinates in grid
    {
        if (0 <= x && x < _width && 0 <= y && y < _height)
        {
            return _gridArray[x, y];
        }
        else
        {
            Debug.LogWarning("Index is outside of array boundaries");
            return default(TGridObject);
        }
    }
}
