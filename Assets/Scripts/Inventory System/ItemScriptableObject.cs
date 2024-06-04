using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptables/Item Config")]
public class ItemScriptableObject : ScriptableObject
{
    //[SerializeField] private int _id;
    [SerializeField] private string _name;
    [SerializeField] private Sprite _iconSprite;

    //public int Id => _id;
    public string Name => _name;
    public Sprite IconSprite => _iconSprite;
}
