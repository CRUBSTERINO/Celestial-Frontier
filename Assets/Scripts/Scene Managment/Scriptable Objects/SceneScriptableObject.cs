using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Scene Config", menuName = "Scriptables/Scene Managment/Scene Config")]
public class SceneScriptableObject : ScriptableObject
{
    [SerializeField] private string _name;
    [SerializeField] private int _buildIndex;
    [SerializeField] private SceneType _sceneType;
    // Scenes that can be reached from this scene
    [SerializeField] private List<SceneScriptableObject> _reachableScenes;
    [Space] 
    [SerializeField] private AudioClip _ambientAudioClip;

    public string Name => _name;
    public int BuildIndex => _buildIndex;
    public SceneType SceneType => _sceneType;
    public List<SceneScriptableObject> ReachableScenes => _reachableScenes;
    public AudioClip AmbientAudioClip => _ambientAudioClip;
}
