using UnityEngine;

namespace DialogSystem.Conditions
{
    // Gives access to the dialogue if the following conditions are met: the player has completed the specified task and the next specified task is not active or completed
    public class TaskAssignmentConditions : DialogLinePlayerCoupledCondition
    {
        [SerializeField] private TaskScriptableObject _completedTask; // Task that should be completed in order to allow access to the next task
        [SerializeField] private TaskScriptableObject _taskToBeAssigned; // Task that shouldn't be completed or active

        public override bool IsConditionFulfilled(DialogLine dialogLine)
        {
            GetPlayerManager(dialogLine);

            TaskManager taskManager = _playerManager.TaskManager;

            if (!taskManager.HasCompletedTask(_completedTask)) return false; // Task that should be completed is not completed => false

            if (taskManager.IsTaskActive(_taskToBeAssigned)) return false; // Task that should be assigned is active => false

            if (taskManager.HasCompletedTask(_taskToBeAssigned)) return false; // Task that should be assigned is already completed => false

            return true;
        }

        public override DialogLineCondition CloneCondition()
        {
            TaskAssignmentConditions clonedCondtion = new TaskAssignmentConditions()
            {
                _completedTask = _completedTask,
                _taskToBeAssigned = _taskToBeAssigned
            };

            return clonedCondtion;
        }
    } 
}
