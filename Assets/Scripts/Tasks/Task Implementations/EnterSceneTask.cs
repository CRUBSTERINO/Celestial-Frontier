using UnityEngine;

// Task is fulfilled when player enters target scene
public class EnterSceneTask : Task
{
    [SerializeField] private SceneScriptableObject _scene;

    private SceneManagmentService _sceneManagmentService;

    private void Awake()
    {
        _sceneManagmentService = ServiceLocator.Instance.GetService<SceneManagmentService>();
    }

    private void OnGameplaySceneLoadedHandler(SceneEntranceScriptableObject sceneEntrance)
    {
        TryComplete();
    }

    protected override bool AreCompletionRequirmentsFulfilled()
    {
        return _sceneManagmentService.CurrentScene == _scene;
    }

    protected override void SubscribeToCompletionRequirments()
    {
        _sceneManagmentService.OnGameplaySceneLoaded += OnGameplaySceneLoadedHandler;
    }

    protected override void UnsubscribeFromCompletionRequirments()
    {
        _sceneManagmentService.OnGameplaySceneLoaded -= OnGameplaySceneLoadedHandler;
    }
}
