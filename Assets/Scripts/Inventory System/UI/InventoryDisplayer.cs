using System.Collections.Generic;
using UnityEngine;

public class InventoryDisplayer : MonoBehaviour
{
    [SerializeField] private GameObject _slotsRowPrefab;
    [SerializeField] private GameObject _slotPrefab;
    [SerializeField] private Transform _rowsParent;
    [Space]
    [SerializeField] private int _slotsInRowCount;

    private Inventory _inventory;
    private List<GameObject> _rows;
    private List<InventorySlotUI> _slots;

    private void SetupUI()
    {
        DestroyUI();

        int rowCount = Mathf.CeilToInt((float)_inventory.Capacity / _slotsInRowCount);
        _rows = new List<GameObject>(rowCount);
        _slots = new List<InventorySlotUI>(rowCount * _slotsInRowCount);

        for (int i = 0; i < rowCount; i++)
        {
            GameObject row = Instantiate(_slotsRowPrefab, _rowsParent);
            _rows.Add(row);

            for (int j = 0; j < _slotsInRowCount; j++)
            {
                InventorySlotUI slot = Instantiate(_slotPrefab, row.transform).GetComponent<InventorySlotUI>();
                _slots.Add(slot);
            }
        }
    }

    private void UpdateUI()
    {
        List<ItemScriptableObject> items = _inventory.Items;

        for (int i = 0; i < _slots.Count; i++)
        {
            InventorySlotUI slot = _slots[i];

            if (i < items.Count)
            {
                if (items[i] != null)
                {
                    slot.SetOccupied(items[i]);
                }
                else
                {
                    slot.SetEmpty();
                }
            }
            else
            {
                slot.SetBlocked();
            }
        }
    }

    private void DestroyUI()
    {
        if (_rows != null)
        {
            foreach (GameObject row in _rows)
            {
                Destroy(row.gameObject);
            }

            _rows.Clear();
            _slots.Clear();
        }
    }

    public void AssignInventory(Inventory inventory)
    {
        if (_inventory != null)
        {
            _inventory.OnItemsUpdated -= UpdateUI;
        }

        _inventory = inventory;

        SetupUI();
        UpdateUI();

        _inventory.OnItemsUpdated += UpdateUI;
    }
}
