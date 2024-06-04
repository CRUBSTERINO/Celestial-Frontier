using UnityEngine;

[CreateAssetMenu(fileName = "Scene Managment Service Config", menuName = "Scriptables/Scene Managment/Scene Managment Service Config")]
public class SceneManagmentScriptableObject : ScriptableObject
{
    [SerializeField] public SceneScriptableObject InitialScene;
    [SerializeField] public GameObject LoadingScreenPrefab;
}