using UnityEngine;

namespace DialogSystem.Actions
{
    public class GiveItemDialogAction : DialogAction
    {
        [SerializeField] private ItemScriptableObject _item;

        // This dialog action should be given only PLAYER AS PERSON
        public override void Trigger(Person person)
        {
            if (person is not PlayerManager)
            {
                Debug.LogWarning("Task can only be assigned to player! Given person isn't player");
            }

            PlayerManager playerManager = person as PlayerManager;
            playerManager.TryToPickUpItem(_item);

            OnActionTriggered();
        }

        public override DialogAction CloneAction()
        {
            GiveItemDialogAction giveItemAction = new GiveItemDialogAction()
            {
                _item = _item
            };

            return giveItemAction;
        }
    } 
}
