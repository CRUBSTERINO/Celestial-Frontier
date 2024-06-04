using UnityEngine;

public class PathfindingAreaSquare : PathfindingArea
{
    [SerializeField] private Vector3Int _extends;
    [SerializeField] private Vector3Int _position;

    public override void ConfigureAffectedPathNodes()
    {
        Grid<PathfindingNode> grid = _affectedSurface.Grid;
        float cellSize = _affectedSurface.Grid.CellSize;

        for (float x = _position.x; x < _position.x + _extends.x; x += cellSize)
        {
            for (float y = _position.y; y < _position.y + _extends.y; y += cellSize)
            {
                int gridX, gridY;
                grid.GetGridPosition(new Vector3(x, y), out gridX, out gridY);
                PathfindingNode pathNode = grid.GetGridObject(gridX, gridY);

                ConfigurePathNode(pathNode);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (_affectedSurface == null) return;
        Gizmos.color = Color.red;
        Vector3 size = (Vector3)_extends * _affectedSurface.CellSize;
        Vector3 position = (Vector3)_extends * 0.5f + _position;

        Gizmos.DrawWireCube(position, size);
    }
}
