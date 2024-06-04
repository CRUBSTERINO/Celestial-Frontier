using System.Collections.Generic;
using UnityEngine;

public class PathfindingPath
{
    private List<Vector3> _waypoints;
    private int _currentWaypointIndex;

    public PathfindingPath(List<Vector3> positions)
    {
        _waypoints = positions;
    }

    public bool TryGetNextWaypoint(out Vector3 nextWaypoint)
    {
        if (_currentWaypointIndex < _waypoints.Count - 1)
        {
            _currentWaypointIndex++;
            nextWaypoint = _waypoints[_currentWaypointIndex];
            return true;
        }
        else
        {
            nextWaypoint = Vector3.zero;
            return false;
        }
    }

    public List<Vector3> GetPositions()
    {
        return _waypoints;
    }

    public void Inverse()
    {
        _waypoints.Reverse();
    }
}
