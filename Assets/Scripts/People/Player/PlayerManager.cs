using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DialogSystem;
using DialogSystem.UI;
using System;

public class PlayerManager : Person
{
    [SerializeField] private int _inventoryCapacity;
    [SerializeField] private Interactor _interactor;
    [SerializeField] private PlayerController _controller;
    [SerializeField] private TaskManager _taskManager;
    [SerializeField] private InventoryDisplayer _inventoryDisplayer;
    [SerializeField] private WorldTimeClocksUI _clocksUI;
    [SerializeField] private DialogBoxUI _dialogBoxUI;
    [Space, Header("UI Pop Up's")]
    [SerializeField] private PopUpUI _inventoryPopUp;
    [SerializeField] private PopUpUI _clocksPopUp;
    [SerializeField] private PopUpUI _dialogBoxPopUp;
    [SerializeField] private PopUpUI _tasksPopUp;

    private Inventory _inventory;
    private DialogManager _activeDialogManager;
    private PlayerInputActions _inputActions;
    private SpriteRenderer _spriteRenderer;
    private Collider2D _collider;
    private bool _isInInventory;
    private bool _isInDialog;
    private bool _isMovementBlocked;
    private bool _isHidden;

    public Inventory Inventory => _inventory;
    public TaskManager TaskManager => _taskManager;
    public SpriteRenderer SpriteRenderer => _spriteRenderer;
    public bool IsHidden => _isHidden;

    public event Action OnAttacked;
    public event Action OnInteractKeyPressed;
    public event Action OnHidden;
    public event Action OnUnhidden;

    private void Start()
    {
        _inventory = new Inventory(new List<ItemScriptableObject>(), _inventoryCapacity, true);
        _inventoryDisplayer.AssignInventory(_inventory);

        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _collider = GetComponentInChildren<Collider2D>();

        _inputActions = new PlayerInputActions();
        _inputActions.Enable();
        _inputActions.GeneralControls.Inventory.performed += InventoryOpenInputHandler;
        _inputActions.GeneralControls.Inventory.canceled += InventoryCloseInputHandler;
        _inputActions.GeneralControls.Interact.performed += InteractInputHandler;

        VerifyMovementAbility();
    }

    private void OnDestroy()
    {
        _inputActions.Disable();
        _inputActions.GeneralControls.Inventory.performed -= InventoryOpenInputHandler;
        _inputActions.GeneralControls.Inventory.canceled -= InventoryCloseInputHandler;
        _inputActions.GeneralControls.Interact.performed -= InteractInputHandler;
    }

    private void InventoryOpenInputHandler(InputAction.CallbackContext context)
    {
        _inventoryPopUp.EnablePopUp();
        _clocksPopUp.EnablePopUp();
        _tasksPopUp.EnablePopUp();
        _isInInventory = true;

        VerifyMovementAbility();
    }

    private void InventoryCloseInputHandler(InputAction.CallbackContext context)
    {
        _inventoryPopUp.DisablePopUp();
        _clocksPopUp.DisablePopUp();
        _tasksPopUp.DisablePopUp();
        _isInInventory = false;

        VerifyMovementAbility();
    }

    private void DialogChainContinueInputHandler(InputAction.CallbackContext context)
    {
        if (_activeDialogManager != null)
        {
            _activeDialogManager.ContinueDialogChain();
        }
    }

    private void InteractInputHandler(InputAction.CallbackContext context)
    {
        OnInteractKeyPressed?.Invoke();

        if (_interactor.IsEnabled)
        {
            _interactor.Interact(); 
        }
    }

    private void LeaveDialog() // Leave dialog when the dialog is finished by the DialogConfigurator
    {
        _dialogBoxPopUp.DisablePopUp();
        _activeDialogManager.OnDialogLeave -= LeaveDialog;
        _isInDialog = false;

        _inputActions.DialogControls.NextSentence.performed -= DialogChainContinueInputHandler;

        VerifyMovementAbility();
    }

    private void VerifyMovementAbility()
    {
        if (_isInInventory || _isInDialog || _isMovementBlocked || _isHidden) // Добавлять условия через ||, которые запрещают движение игрока
        {
            _controller.CanMove = false;
        }
        else
        {
            _controller.CanMove = true;
        }
    }

    public void InitiateDialog(DialogManager dialogManager)
    {
        _activeDialogManager = dialogManager;
        _dialogBoxPopUp.EnablePopUp();
        _dialogBoxUI.AssignDialogManager(_activeDialogManager);
        _activeDialogManager.OnDialogLeave += LeaveDialog;
        _isInDialog = true;

        _inputActions.DialogControls.NextSentence.performed += DialogChainContinueInputHandler;

        VerifyMovementAbility();

        dialogManager.RegisterDialogParticipant(this);
        dialogManager.InitiateDialog();
    }

    public bool TryToPickUpItem(ItemScriptableObject item)
    {
        return _inventory.TryAddItem(item);
    }

    public void FreezePlayer()
    {
        _isMovementBlocked = true;
        VerifyMovementAbility();
    }

    public void UnfreezePlayer()
    {
        _isMovementBlocked = false;
        VerifyMovementAbility();
    }

    public void GetAttacked()
    {
        OnAttacked?.Invoke();
    }

    public void Hide()
    {
        _isHidden = true;
        _spriteRenderer.enabled = false;
        _collider.enabled = false;

        VerifyMovementAbility();

        _interactor.Disable();

        OnHidden?.Invoke();
    }

    public void Unhide()
    {
        _isHidden = false;
        _spriteRenderer.enabled = true;
        _collider.enabled = true;

        VerifyMovementAbility();

        _interactor.Enable();

        OnUnhidden?.Invoke();
    }
}
