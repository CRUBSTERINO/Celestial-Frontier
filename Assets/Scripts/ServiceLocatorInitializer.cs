using System.Collections.Generic;
using UnityEngine;

public class ServiceLocatorInitializer : MonoBehaviour
{
    [SerializeField] private OpenAIServiceScriptableObject _openAiConfig;
    [Space]
    [SerializeField] private GameManagerScriptableObject _gameManagerConfig;
    [Space]
    [SerializeField] private SceneManagmentScriptableObject _sceneManagmentConfig;
    [Space]
    [SerializeField] private WorldTimeServiceScriptableObject _worldTimeConfig;
    [Space]
    [SerializeField] private NPCsScheduleServiceScriptableObject _npcsScheduleConfig;
    [Space]
    [SerializeField] private MinigamesServiceScriptableObject _minigamesConfig;
    [Space]
    [SerializeField] private AudioServiceScriptableObject _audioConfig;

    private ServiceLocator _serviceLocator;
    private bool _isDuplicate;

    private void Awake()
    {
        if (ServiceLocator.IsInitialized)
        {
            _isDuplicate = true;
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject); 

        _serviceLocator = ServiceLocator.Instance;
    }

    private void OnDestroy()
    {
        if (_isDuplicate) return;

        List<IService> services = _serviceLocator.GetAllServices();

        foreach (IService service in services)
        {
            service.OnDestroy();
        }
    }

    public void Initialize()
    {
        // Add services that should be registered in this list (config if needed)
        List<IService> servicesToRegister = new List<IService>()
        {
            new OpenAIService(_openAiConfig),
            new GameManager(_gameManagerConfig),
            new SceneManagmentService(_sceneManagmentConfig),
            new PathfindingService(),
            new WorldTimeService(_worldTimeConfig),
            new NPCSpawnerService(_npcsScheduleConfig),
            new SceneExitFinderService(),
            new TaskService(),
            new MinigamesService(_minigamesConfig),
            new AudioService(_audioConfig)
        };

        foreach (IService service in servicesToRegister)
        {
            _serviceLocator.RegisterService(service);
        }

        // Firstly we register the services and only than we call start method on each of them.
        foreach (IService service in servicesToRegister)
        {
            service.OnStart();
        }

        ServiceLocator.IsInitialized = true;
    }
}
