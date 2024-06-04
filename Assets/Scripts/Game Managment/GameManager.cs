using Cinemachine;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : IService
{
    private GameManagerScriptableObject _config;
    private SceneManagmentService _sceneManagmentService;
    private WorldTimeService _worldTimeService;
    private NPCSpawnerService _npcSpawnerService;
    private PlayerManager _playerManagerInstance;
    private GameObject _cameraInstance;
    private PersonSettings _playerSettings;

    public PlayerManager PlayerManager => _playerManagerInstance;

    public GameManager(GameManagerScriptableObject config)
    {
        _config = config;
    }

    private void InstantiatePlayer(Vector2 position)
    {
        if (_playerManagerInstance == null)
        {
            _playerManagerInstance = Object.Instantiate(_config.PlayerPrefab, position, Quaternion.identity).GetComponent<PlayerManager>();

            _playerManagerInstance.OnAttacked += LoseInGame;

            Object.DontDestroyOnLoad(_playerManagerInstance.gameObject);

            _playerManagerInstance.SetSettings(_playerSettings);
        }
        else
        {
            _playerManagerInstance.transform.position = position;
        }

        if (_cameraInstance == null)
        {
            InstantiateCamera();
        }
    }

    private void DestroyPlayerInstance()
    {
        if (_playerManagerInstance == null) return;

        _playerManagerInstance.OnAttacked -= LoseInGame;

        Object.Destroy(_playerManagerInstance.transform.root.gameObject);
    }

    private void InstantiateCamera()
    {
        if (_playerManagerInstance == null)
        {
            Debug.LogWarning("Player is not instantiated. You should instantiate camera after the player is instantiated");
            return;
        }

        _cameraInstance = Object.Instantiate(_config.CameraPrefab);
        CinemachineVirtualCamera virtualPlayerCamera = _cameraInstance.GetComponentInChildren<CinemachineVirtualCamera>();
        virtualPlayerCamera.Follow = _playerManagerInstance.transform;

        Object.DontDestroyOnLoad(_cameraInstance.gameObject);
    }

    private void DestroyCameraInstance()
    {
        if (_cameraInstance == null) return;

        Object.Destroy(_cameraInstance);
    }

    private void GameplaySceneLoadingStartedHandler(SceneEntranceScriptableObject sceneEntranceConfig)
    {
        _worldTimeService.FreezeTime();
    }

    private void SetupGameplayScene(SceneEntranceScriptableObject sceneEntranceConfig)
    {
        if (sceneEntranceConfig.SceneConfig.SceneType != SceneType.Gameplay)
        {
            Debug.LogWarning("Current scene is not game scene, but you are trying to setup it here");
            return;
        }

        InstantiatePlayer(sceneEntranceConfig.EntrancePosition);
        _npcSpawnerService.SetActiveScene(sceneEntranceConfig.SceneConfig);

        _worldTimeService.UnfreezeTime();
    }

    private void SetupMenuScene(SceneEntranceScriptableObject sceneEntranceConfig)
    {
        DestroyPlayerInstance();
        DestroyCameraInstance();

        _worldTimeService.FreezeTime();
    }

    public void SetPlayerSettings(PersonSettings personSettings)
    {
        _playerSettings = personSettings;
    }

    public void OnStart()
    {
        _sceneManagmentService = ServiceLocator.Instance.GetService<SceneManagmentService>();
        _worldTimeService = ServiceLocator.Instance.GetService<WorldTimeService>();
        _npcSpawnerService = ServiceLocator.Instance.GetService<NPCSpawnerService>();

        _sceneManagmentService.OnGameplaySceneLoaded += SetupGameplayScene;
        _sceneManagmentService.OnGameplaySceneLoadingStarted += GameplaySceneLoadingStartedHandler;
        _sceneManagmentService.OnMenuSceneLoadingStarted += SetupMenuScene;

        if (_sceneManagmentService.CurrentScene.SceneType == SceneType.Menu)
        {
            _worldTimeService.FreezeTime();
        }
        else
        {
            _worldTimeService.UnfreezeTime();
        }
    }

    public void OnDestroy()
    {
        _sceneManagmentService.OnGameplaySceneLoaded -= SetupGameplayScene;
        _sceneManagmentService.OnMenuSceneLoadingStarted -= SetupMenuScene;
        _sceneManagmentService.OnGameplaySceneLoadingStarted -= GameplaySceneLoadingStartedHandler;
    }

    public void LoseInGame()
    {
        _sceneManagmentService.LoadScene(_config.DefeatScreenSceneEntrance);
    }
}
