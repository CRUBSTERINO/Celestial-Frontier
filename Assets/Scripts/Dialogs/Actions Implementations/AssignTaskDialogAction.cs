using UnityEngine;

namespace DialogSystem.Actions
{
    public class AssignTaskDialogAction : DialogAction
    {
        [SerializeField] private TaskScriptableObject _taskConfig;

        // This dialog action should be given only PLAYER AS PERSON
        public override void Trigger(Person person)
        {
            if (person is not PlayerManager)
            {
                Debug.LogWarning("Task can only be assigned to player! Given person isn't player");
            }

            PlayerManager playerManager = person as PlayerManager;
            playerManager.TaskManager.AssignTask(_taskConfig);

            OnActionTriggered();
        }

        public override DialogAction CloneAction()
        {
            AssignTaskDialogAction taskAction = new AssignTaskDialogAction()
            {
                _taskConfig = _taskConfig
            };

            return taskAction;
        }
    } 
}
