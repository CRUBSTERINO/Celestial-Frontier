using System;
using UnityEngine;

// Task instance that is configured in prefab.
// For scene-related tasks "UniqueIndentifier" should be used.
// Task starts to listen to completion requirments after activation
// Task implementations can configure themselves in Awake (usually before activation, should choose to get needed data) or Start (usually after activation, should choose if actions affect completion status) methods.
public abstract class Task : MonoBehaviour
{
    [SerializeField] private TaskScriptableObject _config;
    // Config of next task that should be assigned when this is completed
    // Can be left unassigned if no next task needed
    [SerializeField] private TaskScriptableObject _nextTaskConfig;

    public TaskScriptableObject Config => _config;
    public TaskScriptableObject NextTaskConfig => _nextTaskConfig;

    public event Action<Task> OnCompleted;

    protected abstract bool AreCompletionRequirmentsFulfilled();

    // Here inherited members should realize subscription to completion requitments and subscribe "TryComplete" to them
    protected abstract void SubscribeToCompletionRequirments();

    // Unsubscribe from all completion requitments
    protected abstract void UnsubscribeFromCompletionRequirments();

    protected void TryComplete()
    {
        if (AreCompletionRequirmentsFulfilled())
        {
            OnCompleted?.Invoke(this);
            Deactivate();
        }
    }

    // Activate this task so it can be completed
    public void Activate()
    {
        SubscribeToCompletionRequirments();
    }

    // Set task is inactive so it can't be completed
    public void Deactivate()
    {
        UnsubscribeFromCompletionRequirments();
        Destroy(gameObject); // Now deactivation destroys whole task, but it might change
    }

    // Forcing comletion of task ignoring requirments
    public void ForceComplete()
    {
        OnCompleted?.Invoke(this);
        Deactivate();
    }
}
