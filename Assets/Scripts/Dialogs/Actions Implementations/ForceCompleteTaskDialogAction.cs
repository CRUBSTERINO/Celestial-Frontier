using UnityEngine;

namespace DialogSystem.Actions
{
    public class ForceCompleteTaskDialogAction : DialogAction
    {
        // Task that should be force-completed
        [SerializeField] private TaskScriptableObject _task;

        public override void Trigger(Person person)
        {
            if (person is not PlayerManager)
            {
                Debug.LogWarning("Can only force-complete task when provided person is player! Given person isn't player");
            }

            PlayerManager playerManager = person as PlayerManager;
            playerManager.TaskManager.CompleteTask(_task);

            OnActionTriggered();
        }

        public override DialogAction CloneAction()
        {
            ForceCompleteTaskDialogAction action = new ForceCompleteTaskDialogAction()
            {
                _task = _task
            };

            return action;
        }
    }
}
