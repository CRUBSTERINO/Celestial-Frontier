//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.6.3
//     from Assets/Input System/PlayerInputActions.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @PlayerInputActions: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerInputActions()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerInputActions"",
    ""maps"": [
        {
            ""name"": ""GeneralControls"",
            ""id"": ""8d56821d-c919-4bdd-8512-49d0c52972ac"",
            ""actions"": [
                {
                    ""name"": ""Movement"",
                    ""type"": ""Value"",
                    ""id"": ""3e5e9e28-083a-4108-9ac3-9d8d13f70cfb"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Interact"",
                    ""type"": ""Button"",
                    ""id"": ""2318fb1a-3c01-4ace-ace7-a452077da166"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Inventory"",
                    ""type"": ""Button"",
                    ""id"": ""c04a091c-b09b-484f-81e4-ee72100cfad4"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""27c7abff-379a-47b7-bf1a-7d410a6d1f16"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""d8d61f4e-d57a-45ce-b1a2-4c4d161bdf6b"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""c2b4445b-3b67-4bd2-a4d8-027d6db9ba10"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""87733038-c3d8-4732-b10a-6ab740b55249"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""f0171d5d-a537-488a-aeeb-9d575e060a63"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""e3739cd2-858f-40f0-a768-e2090710c065"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""89ff1f96-cee9-4a12-8c59-e08b87e26fec"",
                    ""path"": ""<Keyboard>/tab"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Inventory"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""DialogControls"",
            ""id"": ""ea26764e-2c0c-440c-b848-43f0a44d5109"",
            ""actions"": [
                {
                    ""name"": ""NextSentence"",
                    ""type"": ""Button"",
                    ""id"": ""76e867ee-4ef0-49cc-bdf8-0502353a041f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""c86ef8ee-09b9-42be-bf7d-69ea7973dbbb"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""NextSentence"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // GeneralControls
        m_GeneralControls = asset.FindActionMap("GeneralControls", throwIfNotFound: true);
        m_GeneralControls_Movement = m_GeneralControls.FindAction("Movement", throwIfNotFound: true);
        m_GeneralControls_Interact = m_GeneralControls.FindAction("Interact", throwIfNotFound: true);
        m_GeneralControls_Inventory = m_GeneralControls.FindAction("Inventory", throwIfNotFound: true);
        // DialogControls
        m_DialogControls = asset.FindActionMap("DialogControls", throwIfNotFound: true);
        m_DialogControls_NextSentence = m_DialogControls.FindAction("NextSentence", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // GeneralControls
    private readonly InputActionMap m_GeneralControls;
    private List<IGeneralControlsActions> m_GeneralControlsActionsCallbackInterfaces = new List<IGeneralControlsActions>();
    private readonly InputAction m_GeneralControls_Movement;
    private readonly InputAction m_GeneralControls_Interact;
    private readonly InputAction m_GeneralControls_Inventory;
    public struct GeneralControlsActions
    {
        private @PlayerInputActions m_Wrapper;
        public GeneralControlsActions(@PlayerInputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Movement => m_Wrapper.m_GeneralControls_Movement;
        public InputAction @Interact => m_Wrapper.m_GeneralControls_Interact;
        public InputAction @Inventory => m_Wrapper.m_GeneralControls_Inventory;
        public InputActionMap Get() { return m_Wrapper.m_GeneralControls; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(GeneralControlsActions set) { return set.Get(); }
        public void AddCallbacks(IGeneralControlsActions instance)
        {
            if (instance == null || m_Wrapper.m_GeneralControlsActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_GeneralControlsActionsCallbackInterfaces.Add(instance);
            @Movement.started += instance.OnMovement;
            @Movement.performed += instance.OnMovement;
            @Movement.canceled += instance.OnMovement;
            @Interact.started += instance.OnInteract;
            @Interact.performed += instance.OnInteract;
            @Interact.canceled += instance.OnInteract;
            @Inventory.started += instance.OnInventory;
            @Inventory.performed += instance.OnInventory;
            @Inventory.canceled += instance.OnInventory;
        }

        private void UnregisterCallbacks(IGeneralControlsActions instance)
        {
            @Movement.started -= instance.OnMovement;
            @Movement.performed -= instance.OnMovement;
            @Movement.canceled -= instance.OnMovement;
            @Interact.started -= instance.OnInteract;
            @Interact.performed -= instance.OnInteract;
            @Interact.canceled -= instance.OnInteract;
            @Inventory.started -= instance.OnInventory;
            @Inventory.performed -= instance.OnInventory;
            @Inventory.canceled -= instance.OnInventory;
        }

        public void RemoveCallbacks(IGeneralControlsActions instance)
        {
            if (m_Wrapper.m_GeneralControlsActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IGeneralControlsActions instance)
        {
            foreach (var item in m_Wrapper.m_GeneralControlsActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_GeneralControlsActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public GeneralControlsActions @GeneralControls => new GeneralControlsActions(this);

    // DialogControls
    private readonly InputActionMap m_DialogControls;
    private List<IDialogControlsActions> m_DialogControlsActionsCallbackInterfaces = new List<IDialogControlsActions>();
    private readonly InputAction m_DialogControls_NextSentence;
    public struct DialogControlsActions
    {
        private @PlayerInputActions m_Wrapper;
        public DialogControlsActions(@PlayerInputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @NextSentence => m_Wrapper.m_DialogControls_NextSentence;
        public InputActionMap Get() { return m_Wrapper.m_DialogControls; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(DialogControlsActions set) { return set.Get(); }
        public void AddCallbacks(IDialogControlsActions instance)
        {
            if (instance == null || m_Wrapper.m_DialogControlsActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_DialogControlsActionsCallbackInterfaces.Add(instance);
            @NextSentence.started += instance.OnNextSentence;
            @NextSentence.performed += instance.OnNextSentence;
            @NextSentence.canceled += instance.OnNextSentence;
        }

        private void UnregisterCallbacks(IDialogControlsActions instance)
        {
            @NextSentence.started -= instance.OnNextSentence;
            @NextSentence.performed -= instance.OnNextSentence;
            @NextSentence.canceled -= instance.OnNextSentence;
        }

        public void RemoveCallbacks(IDialogControlsActions instance)
        {
            if (m_Wrapper.m_DialogControlsActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IDialogControlsActions instance)
        {
            foreach (var item in m_Wrapper.m_DialogControlsActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_DialogControlsActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public DialogControlsActions @DialogControls => new DialogControlsActions(this);
    public interface IGeneralControlsActions
    {
        void OnMovement(InputAction.CallbackContext context);
        void OnInteract(InputAction.CallbackContext context);
        void OnInventory(InputAction.CallbackContext context);
    }
    public interface IDialogControlsActions
    {
        void OnNextSentence(InputAction.CallbackContext context);
    }
}
