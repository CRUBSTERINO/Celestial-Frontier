using System.Collections.Generic;
using UnityEngine;

public class ObtainItemsTask : Task
{
    [SerializeField] private List<ItemScriptableObject> _requiredItems;

    private Inventory _playerInventory;

    private void Awake()
    {
        _playerInventory = FindObjectOfType<PlayerManager>().Inventory; // Task should be instantiated only in scene with PlayerManager (STRONG DEPENDENCY)
    }

    private void Start()
    {
        foreach (ItemScriptableObject item in _requiredItems)
        {
            if (!_playerInventory.ContainsItem(item))
            {
                return;
            }
        }

        TryComplete();
    }

    private void InventoryChangedHandler()
    {
        TryComplete();
    }

    protected override bool AreCompletionRequirmentsFulfilled()
    {
        foreach (ItemScriptableObject item in _requiredItems)
        {
            if (!_playerInventory.ContainsItem(item))
            {
                return false;
            }
        }

        return true;
    }

    protected override void SubscribeToCompletionRequirments()
    {
        _playerInventory.OnItemsUpdated += InventoryChangedHandler;
    }

    protected override void UnsubscribeFromCompletionRequirments()
    {
        _playerInventory.OnItemsUpdated -= InventoryChangedHandler;
    }
}
