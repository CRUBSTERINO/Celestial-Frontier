using UnityEngine;

namespace DialogSystem.Conditions
{
    // To fulfill this condition player has to have the specified task completed
    public class CompletedTaskCondition : DialogLinePlayerCoupledCondition
    {
        [SerializeField] private TaskScriptableObject _taskConfig;

        public override bool IsConditionFulfilled(DialogLine dialogLine)
        {
            GetPlayerManager(dialogLine);

            if (_playerManager.TaskManager.HasCompletedTask(_taskConfig) == _taskConfig)
            {
                return true;
            }
            else
            {
                return false;

            }
        }

        public override DialogLineCondition CloneCondition()
        {
            CompletedTaskCondition clonedCondtion = new CompletedTaskCondition()
            {
                _taskConfig = _taskConfig
            };

            return clonedCondtion;
        }
    } 
}
