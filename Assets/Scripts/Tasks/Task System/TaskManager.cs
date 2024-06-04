using System;
using System.Collections.Generic;
using UnityEngine;

// Controls the execution, assignment and deletion of player tasks.
// Should be placed on player.
public class TaskManager : MonoBehaviour
{
    private List<Task> _activeTasks;
    private List<TaskScriptableObject> _completedTaskConfigs;
    private TaskService _taskService;

    public List<Task> ActiveTasks => _activeTasks;

    public event Action<Task> OnTaskCompleted;
    public event Action<Task> OnTaskAssigned;

    private void Awake()
    {
        _completedTaskConfigs = new List<TaskScriptableObject>();
        _activeTasks = new List<Task>();
    }

    private void Start()
    {
        _taskService = ServiceLocator.Instance.GetService<TaskService>();
    }

    private void ActiveTaskCompletedHandler(Task task)
    {
        task.OnCompleted -= ActiveTaskCompletedHandler;

        if (!_completedTaskConfigs.Contains(task.Config))
        {
            _completedTaskConfigs.Add(task.Config); 
        }

        if (task.NextTaskConfig != null)
        {
            AssignTask(task.NextTaskConfig);
        }
        else
        {
            _activeTasks.Remove(task);
        }

        OnTaskCompleted?.Invoke(task);
    }

    public void AssignTask(TaskScriptableObject taskConfig)
    {
/*        // Deactivate previous task
        if (_activeTasks != null)
        {
            _activeTasks.Deactivate();
            _activeTasks.OnCompleted -= ActiveTaskCompletedHandler;
        }*/

        Task task = _taskService.GetTaskInstance(taskConfig);

        task.OnCompleted += ActiveTaskCompletedHandler;
        task.Activate();

        _activeTasks.Add(task);

        OnTaskAssigned?.Invoke(task);
    }

    public bool HasCompletedTask(TaskScriptableObject taskConfig)
    {
        if (taskConfig == null) return true;

        return _completedTaskConfigs.Contains(taskConfig);
    }

    public bool IsTaskActive(TaskScriptableObject taskConfig)
    {
        return _activeTasks.Find(task => task.Config == taskConfig) != null;
    }

    // Adds provided task to completed list
    // If provided task is active, than also handles it as completed active task
    public void CompleteTask(TaskScriptableObject taskConfig)
    {
        Task matchingTaskInstance = _activeTasks?.Find(task => task.Config == taskConfig);

        if (matchingTaskInstance != null)
        {
            matchingTaskInstance.ForceComplete();
        }
        else if (!_completedTaskConfigs.Contains(taskConfig))
        {
            _completedTaskConfigs.Add(taskConfig);
        }
    }
}
