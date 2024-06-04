using UnityEngine;
using UnityEngine.Tilemaps;

public class PathfindingAreaTilemap : PathfindingArea
{
    [SerializeField] private Tilemap _tilemap;

    public override void ConfigureAffectedPathNodes()
    {
        Grid<PathfindingNode> grid = _affectedSurface.Grid;

        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                Vector3 worldPosition = grid.GetWorldPosition(x, y);

                Vector3Int cellPosition = _tilemap.WorldToCell(worldPosition);

                if (_tilemap.HasTile(cellPosition))
                {
                    PathfindingNode pathNode = grid.GetGridObject(x, y);

                    if (pathNode != null)
                    {
                        ConfigurePathNode(pathNode);
                    }
                }
            }
        }
    }
}
