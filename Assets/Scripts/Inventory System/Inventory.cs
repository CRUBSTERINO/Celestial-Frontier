using System;
using System.Collections.Generic;

public class Inventory
{
    private const int _noEmptySlotIndex = -1;

    private List<ItemScriptableObject> _items;
    private int _capacity;
    private bool _hasLimitedCapacity;

    public List<ItemScriptableObject> Items => _items;
    public int Capacity => _capacity;   
    public bool HasLimitedCapacity => _hasLimitedCapacity;

    public event Action OnItemsUpdated;
    public event Action OnItemAdded;
    public event Action OnItemRemoved;

    public Inventory(List<ItemScriptableObject> items, int capacity, bool hasLimitedCapacity)
    {
        _capacity = capacity;
        _hasLimitedCapacity = hasLimitedCapacity;

        _items = new List<ItemScriptableObject>(_capacity);
        _items.AddRange(items);
        for (int i = _items.Count; i < _capacity; i++)
        {
            _items.Add(null);
        }
    }

    private int GetFirstEmptySlot()
    {
        int emptyIndex = _noEmptySlotIndex;

        for (int i = 0; i < _items.Count; ++i)
        {
            if (_items[i] == null)
            {
                emptyIndex = i;
                break;
            }
        }

        return emptyIndex;
    }

    public bool TryAddItem(ItemScriptableObject item)
    {
        int emptySlotIndex = GetFirstEmptySlot();

        if (emptySlotIndex != _noEmptySlotIndex)
        {
            _items[emptySlotIndex] = item;
            OnItemAdded?.Invoke();
            OnItemsUpdated?.Invoke();
            return true;
        }
        else
        {
            return false;
        }
    }

    public void RemoveItem(ItemScriptableObject item)
    {
        if (_items.Contains(item))
        {
            int index =_items.IndexOf(item);
            _items[index] = null;
            OnItemRemoved?.Invoke();
            OnItemsUpdated?.Invoke(); 
        }
    }

    public void RemoveItemAtIndex(int index) 
    {
        if (index >= 0 && index < _items.Count)
        {
            _items[index] = null;
            OnItemRemoved?.Invoke();
            OnItemsUpdated?.Invoke();
        }
    }

    public bool ContainsItem(ItemScriptableObject item)
    {
        return _items.Contains(item);
    }
}
