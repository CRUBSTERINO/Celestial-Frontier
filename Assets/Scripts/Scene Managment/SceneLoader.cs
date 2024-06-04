using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private SceneEntranceScriptableObject _targetSceneEntrance;

    private SceneManagmentService _sceneManagment;

    public SceneEntranceScriptableObject TargetSceneEntrance => _targetSceneEntrance;
    public SceneScriptableObject TargetScene => _targetSceneEntrance.SceneConfig;

    public void LoadScene()
    {
        _sceneManagment = ServiceLocator.Instance.GetService<SceneManagmentService>();
        _sceneManagment.LoadScene(_targetSceneEntrance);
    }
}
