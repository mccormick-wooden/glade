//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.3.0
//     from Assets/InputSystem/CharacterPlayerControls/CharacterPlayerControls.inputactions
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
                    ""groups"": ""Gamepad"",
                    ""action"": ""Move"",
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
                    ""id"": ""e776f043-e849-46d0-bcb2-ee7e4779b4e9"",
                    ""path"": ""<Mouse>/leftButton"",
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
                    ""interactions"": ""Press(behavior=2)"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Shield"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d54fed2d-2c8d-4b2d-a6e0-6212b08bd1ab"",
                    ""path"": ""<Mouse>/rightButton"",
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
                },
                {
                    ""name"": """",
                    ""id"": ""2ef2907b-7eab-4b85-81c3-5a9b7f6b737b"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Cast"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""WASD"",
                    ""id"": ""8d617ad1-5dd0-4ae8-8aa5-7fe5bdf1ace5"",
                    ""path"": ""Dpad"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""c387181d-c3dd-40a7-9a59-ad8f44dae7ba"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""up"",
                    ""id"": ""a6929238-4b15-42af-8238-db0a5f57d0be"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""29a90678-22cc-487c-af1d-98e23fb27c9e"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""c4cb0024-0b5f-43c5-a1ea-736959dc688a"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""29346dd7-5968-4313-9820-b22a895e738b"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""b3c8ab51-202e-4da1-b32e-70d39e8c2181"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""6fd85825-6888-4d53-a7ff-ce2f55e5e574"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""70345b3b-d844-4bcc-911d-1b2f0b3a807c"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        },
        {
            ""name"": ""PauseGame"",
            ""id"": ""1dda00a2-148b-4538-8a44-2fa2a0047418"",
            ""actions"": [
                {
                    ""name"": ""PauseGameAction"",
                    ""type"": ""Button"",
                    ""id"": ""4e88761c-2f7d-4d4b-aa2b-ba8f68e7ef82"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""746091c3-4c8c-4017-aaf6-611f590ae756"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PauseGameAction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""569a3d68-d31a-48ac-9c2b-d186b31007e4"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PauseGameAction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""SkipScene"",
            ""id"": ""bdcd5bc2-c8f3-4549-b51c-88f9cfc749b0"",
            ""actions"": [
                {
                    ""name"": ""SkipSceneAction"",
                    ""type"": ""Button"",
                    ""id"": ""d7676dcc-4e5d-4e21-80c6-ac26beca9ea1"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""70504d48-77ab-4b13-992e-09fc5b96bdf0"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SkipSceneAction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""47583645-6ac4-482d-aedc-9642ed132bbd"",
                    ""path"": ""<Keyboard>/b"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SkipSceneAction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""DialogueControl"",
            ""id"": ""a3944762-98a8-45d2-9510-b8d07b903394"",
            ""actions"": [
                {
                    ""name"": ""ProgressDialogueAction"",
                    ""type"": ""Button"",
                    ""id"": ""4f5087c2-be38-439c-a3d8-4e3df5c92ede"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""10ba6aa9-f409-471a-a944-3cef6f04c69d"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ProgressDialogueAction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5530eeec-55ce-45f7-8612-88b9b7bab108"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ProgressDialogueAction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Gamepad"",
            ""bindingGroup"": ""Gamepad"",
            ""devices"": [
                {
                    ""devicePath"": ""<Gamepad>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Mouse&Keyboard"",
            ""bindingGroup"": ""Mouse&Keyboard"",
            ""devices"": [
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Gameplay
        m_Gameplay = asset.FindActionMap("Gameplay", throwIfNotFound: true);
        m_Gameplay_Move = m_Gameplay.FindAction("Move", throwIfNotFound: true);
        m_Gameplay_Slash = m_Gameplay.FindAction("Slash", throwIfNotFound: true);
        m_Gameplay_Shield = m_Gameplay.FindAction("Shield", throwIfNotFound: true);
        m_Gameplay_Cast = m_Gameplay.FindAction("Cast", throwIfNotFound: true);
        // PauseGame
        m_PauseGame = asset.FindActionMap("PauseGame", throwIfNotFound: true);
        m_PauseGame_PauseGameAction = m_PauseGame.FindAction("PauseGameAction", throwIfNotFound: true);
        // SkipScene
        m_SkipScene = asset.FindActionMap("SkipScene", throwIfNotFound: true);
        m_SkipScene_SkipSceneAction = m_SkipScene.FindAction("SkipSceneAction", throwIfNotFound: true);
        // DialogueControl
        m_DialogueControl = asset.FindActionMap("DialogueControl", throwIfNotFound: true);
        m_DialogueControl_ProgressDialogueAction = m_DialogueControl.FindAction("ProgressDialogueAction", throwIfNotFound: true);
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
    private readonly InputAction m_Gameplay_Slash;
    private readonly InputAction m_Gameplay_Shield;
    private readonly InputAction m_Gameplay_Cast;
    public struct GameplayActions
    {
        private @CharacterPlayerControls m_Wrapper;
        public GameplayActions(@CharacterPlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_Gameplay_Move;
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

    // PauseGame
    private readonly InputActionMap m_PauseGame;
    private IPauseGameActions m_PauseGameActionsCallbackInterface;
    private readonly InputAction m_PauseGame_PauseGameAction;
    public struct PauseGameActions
    {
        private @CharacterPlayerControls m_Wrapper;
        public PauseGameActions(@CharacterPlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @PauseGameAction => m_Wrapper.m_PauseGame_PauseGameAction;
        public InputActionMap Get() { return m_Wrapper.m_PauseGame; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PauseGameActions set) { return set.Get(); }
        public void SetCallbacks(IPauseGameActions instance)
        {
            if (m_Wrapper.m_PauseGameActionsCallbackInterface != null)
            {
                @PauseGameAction.started -= m_Wrapper.m_PauseGameActionsCallbackInterface.OnPauseGameAction;
                @PauseGameAction.performed -= m_Wrapper.m_PauseGameActionsCallbackInterface.OnPauseGameAction;
                @PauseGameAction.canceled -= m_Wrapper.m_PauseGameActionsCallbackInterface.OnPauseGameAction;
            }
            m_Wrapper.m_PauseGameActionsCallbackInterface = instance;
            if (instance != null)
            {
                @PauseGameAction.started += instance.OnPauseGameAction;
                @PauseGameAction.performed += instance.OnPauseGameAction;
                @PauseGameAction.canceled += instance.OnPauseGameAction;
            }
        }
    }
    public PauseGameActions @PauseGame => new PauseGameActions(this);

    // SkipScene
    private readonly InputActionMap m_SkipScene;
    private ISkipSceneActions m_SkipSceneActionsCallbackInterface;
    private readonly InputAction m_SkipScene_SkipSceneAction;
    public struct SkipSceneActions
    {
        private @CharacterPlayerControls m_Wrapper;
        public SkipSceneActions(@CharacterPlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @SkipSceneAction => m_Wrapper.m_SkipScene_SkipSceneAction;
        public InputActionMap Get() { return m_Wrapper.m_SkipScene; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(SkipSceneActions set) { return set.Get(); }
        public void SetCallbacks(ISkipSceneActions instance)
        {
            if (m_Wrapper.m_SkipSceneActionsCallbackInterface != null)
            {
                @SkipSceneAction.started -= m_Wrapper.m_SkipSceneActionsCallbackInterface.OnSkipSceneAction;
                @SkipSceneAction.performed -= m_Wrapper.m_SkipSceneActionsCallbackInterface.OnSkipSceneAction;
                @SkipSceneAction.canceled -= m_Wrapper.m_SkipSceneActionsCallbackInterface.OnSkipSceneAction;
            }
            m_Wrapper.m_SkipSceneActionsCallbackInterface = instance;
            if (instance != null)
            {
                @SkipSceneAction.started += instance.OnSkipSceneAction;
                @SkipSceneAction.performed += instance.OnSkipSceneAction;
                @SkipSceneAction.canceled += instance.OnSkipSceneAction;
            }
        }
    }
    public SkipSceneActions @SkipScene => new SkipSceneActions(this);

    // DialogueControl
    private readonly InputActionMap m_DialogueControl;
    private IDialogueControlActions m_DialogueControlActionsCallbackInterface;
    private readonly InputAction m_DialogueControl_ProgressDialogueAction;
    public struct DialogueControlActions
    {
        private @CharacterPlayerControls m_Wrapper;
        public DialogueControlActions(@CharacterPlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @ProgressDialogueAction => m_Wrapper.m_DialogueControl_ProgressDialogueAction;
        public InputActionMap Get() { return m_Wrapper.m_DialogueControl; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(DialogueControlActions set) { return set.Get(); }
        public void SetCallbacks(IDialogueControlActions instance)
        {
            if (m_Wrapper.m_DialogueControlActionsCallbackInterface != null)
            {
                @ProgressDialogueAction.started -= m_Wrapper.m_DialogueControlActionsCallbackInterface.OnProgressDialogueAction;
                @ProgressDialogueAction.performed -= m_Wrapper.m_DialogueControlActionsCallbackInterface.OnProgressDialogueAction;
                @ProgressDialogueAction.canceled -= m_Wrapper.m_DialogueControlActionsCallbackInterface.OnProgressDialogueAction;
            }
            m_Wrapper.m_DialogueControlActionsCallbackInterface = instance;
            if (instance != null)
            {
                @ProgressDialogueAction.started += instance.OnProgressDialogueAction;
                @ProgressDialogueAction.performed += instance.OnProgressDialogueAction;
                @ProgressDialogueAction.canceled += instance.OnProgressDialogueAction;
            }
        }
    }
    public DialogueControlActions @DialogueControl => new DialogueControlActions(this);
    private int m_GamepadSchemeIndex = -1;
    public InputControlScheme GamepadScheme
    {
        get
        {
            if (m_GamepadSchemeIndex == -1) m_GamepadSchemeIndex = asset.FindControlSchemeIndex("Gamepad");
            return asset.controlSchemes[m_GamepadSchemeIndex];
        }
    }
    private int m_MouseKeyboardSchemeIndex = -1;
    public InputControlScheme MouseKeyboardScheme
    {
        get
        {
            if (m_MouseKeyboardSchemeIndex == -1) m_MouseKeyboardSchemeIndex = asset.FindControlSchemeIndex("Mouse&Keyboard");
            return asset.controlSchemes[m_MouseKeyboardSchemeIndex];
        }
    }
    public interface IGameplayActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnSlash(InputAction.CallbackContext context);
        void OnShield(InputAction.CallbackContext context);
        void OnCast(InputAction.CallbackContext context);
    }
    public interface IPauseGameActions
    {
        void OnPauseGameAction(InputAction.CallbackContext context);
    }
    public interface ISkipSceneActions
    {
        void OnSkipSceneAction(InputAction.CallbackContext context);
    }
    public interface IDialogueControlActions
    {
        void OnProgressDialogueAction(InputAction.CallbackContext context);
    }
}
