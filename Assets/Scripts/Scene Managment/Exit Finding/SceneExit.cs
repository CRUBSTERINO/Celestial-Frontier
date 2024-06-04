using UnityEngine;

public class SceneExit : SceneLoader
{
    [SerializeField] private Vector3 _exitPosition;
    
    public Vector3 ExitPosition => _exitPosition;
}
