using UnityEngine;

public class ActiveTaskGameObjectSpawner : ConditionalGameObjectSpawner
{
    // Task that should be active to fulfill spawn condition
    [SerializeField] private TaskScriptableObject _activeTask;

    protected override bool AreSpawnConditionsFulfilled()
    {
        GameManager gameManager = ServiceLocator.Instance.GetService<GameManager>();
        TaskManager taskManager = gameManager.PlayerManager.TaskManager;

        return taskManager.IsTaskActive(_activeTask);
    }
}
