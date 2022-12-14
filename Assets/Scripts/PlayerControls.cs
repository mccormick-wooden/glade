//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.3.0
//     from Assets/Controls/PlayerControls.inputactions
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

public partial class @PlayerControls : IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerControls"",
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
                    ""name"": ""CameraLook"",
                    ""type"": ""PassThrough"",
                    ""id"": ""00c80e57-f82f-4817-b150-f2c77d9ccfbc"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""ThrowGravityBall"",
                    ""type"": ""Button"",
                    ""id"": ""883459a1-3f74-4e0a-a366-cd98c6d377e6"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""PlaceGravityCube"",
                    ""type"": ""Button"",
                    ""id"": ""d40c21eb-4c37-43c8-9d47-dd7e0ff6f096"",
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
                    ""action"": ""CameraLook"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f133b195-b71f-442d-a5a8-3b520f4e809c"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ThrowGravityBall"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""fadef10e-8d80-4428-82ae-ce6a9b442cc4"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PlaceGravityCube"",
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
        m_Gameplay_CameraLook = m_Gameplay.FindAction("CameraLook", throwIfNotFound: true);
        m_Gameplay_ThrowGravityBall = m_Gameplay.FindAction("ThrowGravityBall", throwIfNotFound: true);
        m_Gameplay_PlaceGravityCube = m_Gameplay.FindAction("PlaceGravityCube", throwIfNotFound: true);
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
    private readonly InputAction m_Gameplay_CameraLook;
    private readonly InputAction m_Gameplay_ThrowGravityBall;
    private readonly InputAction m_Gameplay_PlaceGravityCube;
    public struct GameplayActions
    {
        private @PlayerControls m_Wrapper;
        public GameplayActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_Gameplay_Move;
        public InputAction @CameraLook => m_Wrapper.m_Gameplay_CameraLook;
        public InputAction @ThrowGravityBall => m_Wrapper.m_Gameplay_ThrowGravityBall;
        public InputAction @PlaceGravityCube => m_Wrapper.m_Gameplay_PlaceGravityCube;
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
                @CameraLook.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnCameraLook;
                @CameraLook.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnCameraLook;
                @CameraLook.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnCameraLook;
                @ThrowGravityBall.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnThrowGravityBall;
                @ThrowGravityBall.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnThrowGravityBall;
                @ThrowGravityBall.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnThrowGravityBall;
                @PlaceGravityCube.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnPlaceGravityCube;
                @PlaceGravityCube.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnPlaceGravityCube;
                @PlaceGravityCube.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnPlaceGravityCube;
            }
            m_Wrapper.m_GameplayActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
                @CameraLook.started += instance.OnCameraLook;
                @CameraLook.performed += instance.OnCameraLook;
                @CameraLook.canceled += instance.OnCameraLook;
                @ThrowGravityBall.started += instance.OnThrowGravityBall;
                @ThrowGravityBall.performed += instance.OnThrowGravityBall;
                @ThrowGravityBall.canceled += instance.OnThrowGravityBall;
                @PlaceGravityCube.started += instance.OnPlaceGravityCube;
                @PlaceGravityCube.performed += instance.OnPlaceGravityCube;
                @PlaceGravityCube.canceled += instance.OnPlaceGravityCube;
            }
        }
    }
    public GameplayActions @Gameplay => new GameplayActions(this);
    public interface IGameplayActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnCameraLook(InputAction.CallbackContext context);
        void OnThrowGravityBall(InputAction.CallbackContext context);
        void OnPlaceGravityCube(InputAction.CallbackContext context);
    }
}
