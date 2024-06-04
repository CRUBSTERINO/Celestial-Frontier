using System;
using System.Collections.Generic;
using UnityEngine;

public class Hideout : MonoBehaviour, IInteractable
{
    private const int INTERACTABLE_LAYER = 3;

    [SerializeField] private HideoutInteraction _hideoutInteraction;

    private PlayerManager _playerManager;
    // Position from which player entered the hideout
    private Vector3 _enteredFromPosition;
    private bool _isOccupied;

    public Vector3 InteractionPosition => transform.position;

    public event Action OnInteracted;
    public event Action OnBecameNonInteractable;

    private void OnDestroy()
    {
        if (_playerManager != null)
        {
            _playerManager.OnInteractKeyPressed -= OnPlayerInteractKeyPressedHandler;
        }

        OnBecameNonInteractable?.Invoke();
    }

    private void OnPlayerInteractKeyPressedHandler()
    {
        ExitHideout();
    }

    private void SetAllChildrenLayer(int layer)
    {
        var children = transform.GetComponentsInChildren<Transform>(includeInactive: true);
        foreach (var child in children)
        {
            child.gameObject.layer = layer;
        }
    }

    public void EnterHideout(PlayerManager playerManager)
    {
        _playerManager = playerManager;
        _isOccupied = true;
        _enteredFromPosition = playerManager.transform.position;

        // Make all children to be ignored in RayCast. Used for Enemie's player detection when entering hideout
        SetAllChildrenLayer(2);

        playerManager.Hide();

        _playerManager.transform.position = transform.position;

        playerManager.OnInteractKeyPressed += OnPlayerInteractKeyPressedHandler;
    }

    public void ExitHideout()
    {
        if (!_isOccupied)
        {
            Debug.LogError("Can't exit hideout, it is not occupied");
        }

        _playerManager.OnInteractKeyPressed -= OnPlayerInteractKeyPressedHandler;

        _playerManager.transform.position = _enteredFromPosition;
        _enteredFromPosition = Vector3.zero;

        // Set back default layer
        SetAllChildrenLayer(INTERACTABLE_LAYER);

        _playerManager.Unhide();

        _playerManager = null;
        _isOccupied = false;
    }

    public List<Interaction> GetInteractions()
    {
        return new List<Interaction> { _hideoutInteraction };
    }

    public void PerformInteraction(Interaction interaction, Interactor interactor)
    {
        interaction.PerformInteraction(interactor);
        OnInteracted?.Invoke();
    }
}
