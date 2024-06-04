using System.Collections.Generic;
using UnityEngine;

// Task that requires completion of sub-tasks in order to be completed
public class ComplexTask : Task
{
    // List of sub-tasks that should be completed to complete this complex task
    [SerializeField] private List<TaskScriptableObject> _requiredTasks;

    private TaskManager _taskManager;

    private void Awake()
    {
        _taskManager = FindObjectOfType<TaskManager>();

        if (_taskManager == null)
        {
            Debug.LogError("Wasn't able to find TaskManager in scene");
            return;
        }
    }

    private void Start()
    {
        bool hasCompletedAllTasks = true;

        foreach (TaskScriptableObject taskConfig in _requiredTasks)
        {
            // Assign all uncompleted tasks to TaskManager
            if (!_taskManager.HasCompletedTask(taskConfig))
            {
                hasCompletedAllTasks = false;

                _taskManager.AssignTask(taskConfig);
            }
        }

        if (hasCompletedAllTasks)
        {
            TryComplete();
        }
    }

    private void OnTaskCompletedHandler(Task task)
    {
        TryComplete();
    }

    protected override bool AreCompletionRequirmentsFulfilled()
    {
        foreach (TaskScriptableObject taskConfig in _requiredTasks)
        {
            if (!_taskManager.HasCompletedTask(taskConfig))
            {
                return false;
            }
        }

        return true;
    }

    protected override void SubscribeToCompletionRequirments()
    {
        _taskManager.OnTaskCompleted += OnTaskCompletedHandler;
    }

    protected override void UnsubscribeFromCompletionRequirments()
    {
        _taskManager.OnTaskCompleted -= OnTaskCompletedHandler;
    }
}
