using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;

public class DebuggingCheats : MonoBehaviour
{
    [SerializeField] private TaskScriptableObject _task;
    [Space]
    [SerializeField] private SceneEntranceScriptableObject _sceneEntrance;

    private TaskManager _taskManager;
    private SceneManagmentService _sceneManagmentService;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        _sceneManagmentService = ServiceLocator.Instance.GetService<SceneManagmentService>();
    }

    [Button("Assign Task")]
    private void AssignSelectedTask()
    {
        if (_taskManager == null && !TryFindTaskManagerInScene())
        {
            Debug.LogWarning("Can't assign task. No TaskManager found on scene.");
            return;
        }

        _taskManager.AssignTask(_task);
    }

    [Button("Complete Task")]
    private void CompleteSelectedTask()
    {
        if (_taskManager == null && !TryFindTaskManagerInScene())
        {
            Debug.LogWarning("Can't assign task. No TaskManager found on scene.");
            return;
        }

        _taskManager.CompleteTask(_task);
    }

    [Button("Load Scene")]
    private void LoadScene()
    {
        _sceneManagmentService.LoadScene(_sceneEntrance);
    }

    [Button("Call Minigame")]
    private void CallMinigame()
    {
        ServiceLocator.Instance.GetService<MinigamesService>().RequestDefaultMinigame().Forget();
    }

    private bool TryFindTaskManagerInScene()
    {
        _taskManager = FindObjectOfType<TaskManager>();

        return _taskManager != null;
    }
}
