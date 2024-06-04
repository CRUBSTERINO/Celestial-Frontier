using UnityEngine;

namespace DialogSystem.Conditions
{
    // In order to access the dialogue, the player must have the specified item
    public class ItemPresenceCondition : DialogLinePlayerCoupledCondition
    {
        [SerializeField] private ItemScriptableObject _itemConfig;

        public override bool IsConditionFulfilled(DialogLine dialogLine)
        {
            GetPlayerManager(dialogLine);

            if (_playerManager.Inventory.ContainsItem(_itemConfig))
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
            ItemPresenceCondition clonedCondtion = new ItemPresenceCondition()
            {
                _itemConfig = _itemConfig
            };

            return clonedCondtion;
        }
    } 
}
