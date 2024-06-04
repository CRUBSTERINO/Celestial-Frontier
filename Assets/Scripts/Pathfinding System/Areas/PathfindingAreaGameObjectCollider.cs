using UnityEngine;

// Pathfinding area that affects surface nodes that lay inside of gameObject's collider, which this component has
public class PathfindingAreaGameObjectCollider : PathfindingArea
{
    private Collider2D _collider;
    private Bounds _bounds;

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        _collider = GetComponentInChildren<Collider2D>();
        _bounds = _collider.bounds;
    }

    public override void ConfigureAffectedPathNodes()
    {
        // Can be called before Awake, because surface is baked also in Awake
        if (_collider == null) Initialize();

        Grid<PathfindingNode> grid = _affectedSurface.Grid;
        float cellSize = _affectedSurface.Grid.CellSize;

        for (float x = _bounds.min.x; x <= _bounds.max.x; x += cellSize)
        {
            for (float y = _bounds.min.y; y <= _bounds.max.y; y += cellSize)
            {
                int gridX, gridY;

                grid.GetGridPosition(new Vector3(x, y), out gridX, out gridY);
                PathfindingNode pathNode = grid.GetGridObject(gridX, gridY);

                ConfigurePathNode(pathNode);
            }
        }
    }
}
