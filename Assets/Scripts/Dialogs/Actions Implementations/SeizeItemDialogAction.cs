using UnityEngine;

namespace DialogSystem.Actions
{
    public class SeizeItemDialogAction : DialogAction
    {
        [SerializeField] private ItemScriptableObject _item; // Item to seize from player

        // This dialog action should be given only PLAYER AS PERSON
        public override void Trigger(Person person)
        {
            if (person is not PlayerManager)
            {
                Debug.LogWarning("Task can only be assigned to player! Given person isn't player");
            }

            PlayerManager playerManager = person as PlayerManager;
            playerManager.Inventory.RemoveItem(_item);

            OnActionTriggered();
        }

        public override DialogAction CloneAction()
        {
            SeizeItemDialogAction seizeItemAction = new SeizeItemDialogAction()
            {
                _item = _item
        };

            return seizeItemAction;
        }
    }
}
