using UnityEngine;

namespace DialogSystem.Conditions
{

    // To fulfill this condition for dialog line, a player must not have the specified task as active
    public class InactiveTaskCondition : DialogLinePlayerCoupledCondition
    {
        [SerializeField] private TaskScriptableObject _taskConfig;

        public override bool IsConditionFulfilled(DialogLine dialogLine)
        {
            GetPlayerManager(dialogLine);

            return _playerManager.TaskManager.IsTaskActive(_taskConfig) == false;
        }

        public override DialogLineCondition CloneCondition()
        {
            InactiveTaskCondition clonedCondtion = new InactiveTaskCondition()
            {
                _taskConfig = _taskConfig
            };

            return clonedCondtion;
        }
    } 
}
