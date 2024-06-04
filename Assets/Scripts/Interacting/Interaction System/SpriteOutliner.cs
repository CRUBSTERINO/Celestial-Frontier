using EPOOutline;
using UnityEngine;

[RequireComponent(typeof(Outlinable))]
public class SpriteOutliner : MonoBehaviour
{
    private Outlinable _outlinable;

    private void Awake()
    {
        _outlinable = GetComponent<Outlinable>();
    }

    private void Start()
    {
        DisableOutline();
    }

    public void EnableOutline()
    {
        _outlinable.enabled = true;
    }

    public void DisableOutline()
    {
        _outlinable.enabled = false;
    }
}
