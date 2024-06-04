using System;
using UnityEngine;

[Serializable]
public class DayActivity
{
    [SerializeField] private Vector3 _location;
    [SerializeField] private TimeInterval _interval;
    [SerializeField] private SceneScriptableObject _scene;

    public Vector3 Location => _location;
    public TimeInterval Interval => _interval;
    public SceneScriptableObject Scene => _scene;
}
