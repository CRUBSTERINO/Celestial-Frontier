using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private Color _emptyColor;
    [SerializeField] private Color _occupiedColor;
    [SerializeField] private Color _blockedColor;
    [Space]
    [SerializeField] private Image _backgroundImage;
    [SerializeField] private Image _itemIcon;

    public void SetEmpty()
    {
        _itemIcon.color = _emptyColor;
        _itemIcon.sprite = null;
        _itemIcon.enabled = false;
    }

    public void SetOccupied(ItemScriptableObject item)
    {
        if (item.IconSprite != null)
        {
            _itemIcon.sprite = item.IconSprite;
        }
        else
        {
            _itemIcon.color = _occupiedColor;
        }

        _itemIcon.enabled = true;
    }

    public void SetBlocked()
    {
        _backgroundImage.color = _blockedColor;
        _backgroundImage.enabled = true;
        _itemIcon.enabled = false;
    }
}
