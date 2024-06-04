using UnityEngine;

namespace DialogSystem.Conditions
{
    // To fulfill this condition for dialog line, a player must have the specified task
    public class ActiveTaskCondition : DialogLinePlayerCoupledCondition
    {
        [SerializeField] private TaskScriptableObject _taskConfig;
        
        public override bool IsConditionFulfilled(DialogLine dialogLine)
        {
            GetPlayerManager(dialogLine);

            return _playerManager.TaskManager.IsTaskActive(_taskConfig);
        }

        public override DialogLineCondition CloneCondition()
        {
            ActiveTaskCondition clonedCondition = new ActiveTaskCondition()
            {
                _taskConfig = _taskConfig
            };

            return clonedCondition;
        }
    } 
}
