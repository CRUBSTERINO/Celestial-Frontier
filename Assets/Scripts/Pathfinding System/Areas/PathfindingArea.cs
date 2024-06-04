using NaughtyAttributes;
using UnityEngine;

public abstract class PathfindingArea : MonoBehaviour // Area that affects PathNodes in related PathfindingSurface (makes them unwalkable, changes priority)
{
    [SerializeField, MinValue(1f)] protected int _priority; // 1 - first priority. 
    [SerializeField] protected bool _isWalkable;
    [SerializeField] protected PathfindingSurface _affectedSurface;

    public int Priority => _priority;
    public bool IsWalkable => _isWalkable;

    protected void ConfigurePathNode(PathfindingNode node)
    {
        node.Priority = _priority;
        node.IsWalkable = _isWalkable;
    }

    public bool AffectsSurface(PathfindingSurface surface)
    {
        if (_affectedSurface == surface)
        {
            return true;
        }
        return false;
    }

    public abstract void ConfigureAffectedPathNodes();
}
