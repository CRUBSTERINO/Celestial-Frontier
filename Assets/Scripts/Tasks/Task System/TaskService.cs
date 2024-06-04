using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Is responsible for creating task instances by their config.
public class TaskService : IService
{
    // Path to task prefabs in Resources folder
    private const string TASK_PREFABS_PATH = "Task Prefabs";

    private Dictionary<TaskScriptableObject, Task> _tasksDictionary;

    public TaskService()
    {
        // Load task prefabs from Resources folder
        GameObject[] taskGameObjects = Resources.LoadAll(TASK_PREFABS_PATH, typeof(GameObject)).Cast<GameObject>().ToArray();
        Task[] tasks = new Task[taskGameObjects.Length];

        _tasksDictionary = new Dictionary<TaskScriptableObject, Task>(tasks.Length);

        for (int i = 0; i < taskGameObjects.Length; i++)
        {
            tasks[i] = taskGameObjects[i].GetComponent<Task>();
        }

        foreach (Task task in tasks)
        {
            _tasksDictionary.Add(task.Config, task);
        }
    }

    private Task InstantiateTask(GameObject taskPrefab)
    {
        GameObject instance = Object.Instantiate(taskPrefab);
        Object.DontDestroyOnLoad(instance);
        return instance.GetComponent<Task>();
    }

    public void OnDestroy()
    {

    }

    public void OnStart()
    {

    }

    public Task GetTaskInstance(TaskScriptableObject taskConfig)
    {
        if (_tasksDictionary.ContainsKey(taskConfig))
        {
            return InstantiateTask(_tasksDictionary[taskConfig].transform.root.gameObject);
        }
        else
        {
            Debug.LogWarning("Given task config is not registered in task service: " + taskConfig.ToString());
            return null;
        }
    }
}