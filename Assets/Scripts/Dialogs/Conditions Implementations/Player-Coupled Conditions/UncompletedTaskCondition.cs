using UnityEngine;

namespace DialogSystem.Conditions
{
    // To fulfill this condition have to not complete specified task before
    public class UncompletedTaskCondition : DialogLinePlayerCoupledCondition
    {
        [SerializeField] private TaskScriptableObject _taskConfig;

        public override bool IsConditionFulfilled(DialogLine dialogLine)
        {
            GetPlayerManager(dialogLine);

            if (!_playerManager.TaskManager.HasCompletedTask(_taskConfig) == _taskConfig)
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
            UncompletedTaskCondition clonedCondtion = new UncompletedTaskCondition()
            {
                _taskConfig = _taskConfig
            };

            return clonedCondtion;
        }
    } 
}
