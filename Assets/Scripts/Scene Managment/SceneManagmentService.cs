using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagmentService : IService
{
    private SceneScriptableObject _currentScene;
    private GameObject _loadingScreenPrefab;
    private GameObject _loadingScreenInstance;

    public SceneScriptableObject CurrentScene => _currentScene;

    public event Action<SceneEntranceScriptableObject> OnGameplaySceneLoadingStarted;
    public event Action<SceneEntranceScriptableObject> OnGameplaySceneLoaded;
    public event Action<SceneEntranceScriptableObject> OnMenuSceneLoadingStarted;
    public event Action<SceneEntranceScriptableObject> OnMenuSceneLoaded;

    public SceneManagmentService(SceneManagmentScriptableObject config)
    {
        _currentScene = config.InitialScene;
        _loadingScreenPrefab = config.LoadingScreenPrefab;
    }

    public void OnStart()
    {
        
    }

    public void OnDestroy()
    {

    }

    // Loading screen is instantiated only when loading gameplay scenes
    private void InstantiateLoadingScreen(AsyncOperation loadingTask)
    {
        _loadingScreenInstance = UnityEngine.Object.Instantiate(_loadingScreenPrefab);
        UnityEngine.Object.DontDestroyOnLoad(_loadingScreenInstance);
        _loadingScreenInstance.GetComponent<LoadingScreenUI>().VisualizeSceneLoadingOperation(loadingTask);
    }

    private void DestroyLoadingScreen()
    {
        if (_loadingScreenInstance == null) return;

        UnityEngine.Object.Destroy(_loadingScreenInstance);
    }

    private async void LoadGameplayScene(SceneEntranceScriptableObject sceneEntranceConfig, AsyncOperation loadingTask)
    {
        InstantiateLoadingScreen(loadingTask);
        await loadingTask;
        OnGameplaySceneLoaded?.Invoke(sceneEntranceConfig);
        await UniTask.WaitForSeconds(0.2f); // This small delay is not necessary, but it is there just to show loading screen and make the transition not so fast (game is playable during delay!)
        DestroyLoadingScreen();
    }

    private async void LoadMenuScene(SceneEntranceScriptableObject sceneEntranceConfig, AsyncOperation loadingTask)
    {
        await loadingTask;
        OnMenuSceneLoaded?.Invoke(sceneEntranceConfig);
    }

    public void LoadScene(SceneEntranceScriptableObject sceneEntranceConfig)
    {
        AsyncOperation task = SceneManager.LoadSceneAsync(sceneEntranceConfig.SceneConfig.BuildIndex);
        _currentScene = sceneEntranceConfig.SceneConfig;

        switch (sceneEntranceConfig.SceneConfig.SceneType)
        {
            case SceneType.Gameplay:
                OnGameplaySceneLoadingStarted?.Invoke(sceneEntranceConfig);
                LoadGameplayScene(sceneEntranceConfig, task); 
                break;
            case SceneType.Menu:
                OnMenuSceneLoadingStarted?.Invoke(sceneEntranceConfig);
                LoadMenuScene(sceneEntranceConfig, task); 
                break;
        }
    }
}
