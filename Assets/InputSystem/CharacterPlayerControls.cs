//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.3.0
//     from Assets/InputSystem/CharacterPlayerControls.inputactions
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

public partial class @CharacterPlayerControls : IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @CharacterPlayerControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""CharacterPlayerControls"",
    ""maps"": [
        {
            ""name"": ""Gameplay"",
            ""id"": ""04087c1d-719e-4d21-ab78-94d9aef6554f"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""PassThrough"",
                    ""id"": ""41cfab35-370b-4200-929f-8d7854266d71"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Rotate"",
                    ""type"": ""PassThrough"",
                    ""id"": ""00c80e57-f82f-4817-b150-f2c77d9ccfbc"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Slash"",
                    ""type"": ""Button"",
                    ""id"": ""8c1a5430-3947-4dda-9a47-92514a01bad5"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Shield"",
                    ""type"": ""Button"",
                    ""id"": ""b99a7947-4aae-45f4-b0f6-13c9752fc63d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Cast"",
                    ""type"": ""Button"",
                    ""id"": ""2672c397-b0f9-4571-abbf-8458abee1f37"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""80827b35-481c-4019-9391-6178cf7d3c71"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""356a846a-eed4-487c-9d82-baf9c9023d63"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""68df9930-6d45-412b-b2c9-5331acfa707d"",
                    ""path"": ""<Gamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Slash"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""240698f4-0c44-413a-80ca-576e80132ed5"",
                    ""path"": ""<Gamepad>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Shield"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f1e4228e-5391-454c-8a95-ced59060f846"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Cast"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Gameplay
        m_Gameplay = asset.FindActionMap("Gameplay", throwIfNotFound: true);
        m_Gameplay_Move = m_Gameplay.FindAction("Move", throwIfNotFound: true);
        m_Gameplay_Rotate = m_Gameplay.FindAction("Rotate", throwIfNotFound: true);
        m_Gameplay_Slash = m_Gameplay.FindAction("Slash", throwIfNotFound: true);
        m_Gameplay_Shield = m_Gameplay.FindAction("Shield", throwIfNotFound: true);
        m_Gameplay_Cast = m_Gameplay.FindAction("Cast", throwIfNotFound: true);
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

    // Gameplay
    private readonly InputActionMap m_Gameplay;
    private IGameplayActions m_GameplayActionsCallbackInterface;
    private readonly InputAction m_Gameplay_Move;
    private readonly InputAction m_Gameplay_Rotate;
    private readonly InputAction m_Gameplay_Slash;
    private readonly InputAction m_Gameplay_Shield;
    private readonly InputAction m_Gameplay_Cast;
    public struct GameplayActions
    {
        private @CharacterPlayerControls m_Wrapper;
        public GameplayActions(@CharacterPlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_Gameplay_Move;
        public InputAction @Rotate => m_Wrapper.m_Gameplay_Rotate;
        public InputAction @Slash => m_Wrapper.m_Gameplay_Slash;
        public InputAction @Shield => m_Wrapper.m_Gameplay_Shield;
        public InputAction @Cast => m_Wrapper.m_Gameplay_Cast;
        public InputActionMap Get() { return m_Wrapper.m_Gameplay; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(GameplayActions set) { return set.Get(); }
        public void SetCallbacks(IGameplayActions instance)
        {
            if (m_Wrapper.m_GameplayActionsCallbackInterface != null)
            {
                @Move.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMove;
                @Rotate.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnRotate;
                @Rotate.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnRotate;
                @Rotate.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnRotate;
                @Slash.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnSlash;
                @Slash.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnSlash;
                @Slash.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnSlash;
                @Shield.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnShield;
                @Shield.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnShield;
                @Shield.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnShield;
                @Cast.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnCast;
                @Cast.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnCast;
                @Cast.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnCast;
            }
            m_Wrapper.m_GameplayActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
                @Rotate.started += instance.OnRotate;
                @Rotate.performed += instance.OnRotate;
                @Rotate.canceled += instance.OnRotate;
                @Slash.started += instance.OnSlash;
                @Slash.performed += instance.OnSlash;
                @Slash.canceled += instance.OnSlash;
                @Shield.started += instance.OnShield;
                @Shield.performed += instance.OnShield;
                @Shield.canceled += instance.OnShield;
                @Cast.started += instance.OnCast;
                @Cast.performed += instance.OnCast;
                @Cast.canceled += instance.OnCast;
            }
        }
    }
    public GameplayActions @Gameplay => new GameplayActions(this);
    public interface IGameplayActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnRotate(InputAction.CallbackContext context);
        void OnSlash(InputAction.CallbackContext context);
        void OnShield(InputAction.CallbackContext context);
        void OnCast(InputAction.CallbackContext context);
    }
}
