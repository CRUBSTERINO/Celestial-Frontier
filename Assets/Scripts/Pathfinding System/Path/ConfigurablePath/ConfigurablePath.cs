using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConfigurablePath : MonoBehaviour
{
    [SerializeField] private ConfigurablePathLoopType _loopType;
    [SerializeField] private List<PathCheckpoint> _checkPoints;

    private void Start()
    {
        for (int i = 0; i < _checkPoints.Count; i++)
        {
            _checkPoints[i].Id = i;
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Assign all checkpoints")]
    private void AssignAllCheckpoints()
    {
        _checkPoints = GetComponentsInChildren<PathCheckpoint>().ToList();
    }

    [ContextMenu("Add checkpoint")]
    private void AddCheckpoint()
    {
        Vector3 position = transform.position;

        if (_checkPoints != null)
        {
            position = (_checkPoints != null && _checkPoints.Count > 0) ? _checkPoints[_checkPoints.Count - 1].transform.position : transform.position; 
        }

        PathCheckpoint checkPoint = PathCheckpoint.InstantiateCheckPoint(position);

        checkPoint.transform.SetParent(transform, true);
        checkPoint.transform.SetAsLastSibling();
        checkPoint.gameObject.name = "Checkpoint " + _checkPoints.Count;
        _checkPoints.Add(checkPoint);
    }
#endif

    public PathCheckpoint GetNextCheckpoint(PathCheckpoint previousCheckpoint, PathCheckpoint currentCheckpoint)
    {
        int nextIndex;

        int previousIndex = (previousCheckpoint != null) ? previousCheckpoint.Id : PathCheckpoint.InvalidIdIndex;
        int currentIndex = (currentCheckpoint != null) ? currentCheckpoint.Id : PathCheckpoint.InvalidIdIndex;

        if (currentIndex == PathCheckpoint.InvalidIdIndex)
        {
            nextIndex = 0;
        }
        else // Обычные случаи
        {
            switch (_loopType)
            {
                case ConfigurablePathLoopType.PingPong:
                    if (previousIndex < currentIndex)
                    {
                        nextIndex = (currentIndex + 1 < _checkPoints.Count) ? currentIndex + 1 : currentIndex - 1;
                    }
                    else
                    {
                        nextIndex = (currentIndex - 1 >= 0) ? currentIndex - 1 : currentIndex + 1;
                    }
                    
                    break;

                case ConfigurablePathLoopType.Circle:
                    nextIndex = (currentIndex + 1) % _checkPoints.Count;

                    break;

                default:
                    nextIndex = 0;
                    Debug.LogWarning("Use of this loop type is unsupported");

                    break;
            }
        }

        return _checkPoints[nextIndex];
    }

    public PathCheckpoint GetClosestCheckpoint(Vector3 position)
    {
        int nextPointId = 0;
        for (int i = 0; i < _checkPoints.Count; i++)
        {
            Vector3 distance = _checkPoints[i].Position - position;
            Vector3 nextIdDistance = _checkPoints[nextPointId].Position - position;
            if (distance.sqrMagnitude < nextIdDistance.sqrMagnitude)
            {
                nextPointId = i;
            }
        }

        return _checkPoints[nextPointId];
    }

    private void OnDrawGizmosSelected()
    {
        if (_checkPoints != null)
        {
            Gizmos.color = Color.yellow;
            for (int i = 0; i < _checkPoints.Count; i++)
            {
                Gizmos.DrawSphere(_checkPoints[i].Position, 0.2f);
                if (i > 0)
                {
                    Gizmos.DrawLine(_checkPoints[i].Position, _checkPoints[i - 1].Position);
                }
            } 
        }
    }
}
