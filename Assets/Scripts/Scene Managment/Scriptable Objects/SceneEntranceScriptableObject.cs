using UnityEngine;

[CreateAssetMenu(fileName = "SceneEntranceConfig", menuName = "Scriptables/Scene Managment/Scene Entrance Config")]
public class SceneEntranceScriptableObject : ScriptableObject
{
    [SerializeField] private SceneScriptableObject _sceneConfig;
    [SerializeField] private Vector3 _entrancePosition;

    public SceneScriptableObject SceneConfig => _sceneConfig;
    public Vector3 EntrancePosition => _entrancePosition;
}
