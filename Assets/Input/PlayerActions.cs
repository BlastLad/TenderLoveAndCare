// GENERATED AUTOMATICALLY FROM 'Assets/Input/PlayerActions.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @PlayerActions : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerActions()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerActions"",
    ""maps"": [
        {
            ""name"": ""Tutorial"",
            ""id"": ""8f229dad-2e3f-4747-b567-89207009eb04"",
            ""actions"": [
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""2d251249-a897-4912-a135-23e75b27f1b0"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Slide"",
                    ""type"": ""Button"",
                    ""id"": ""6cb8ae44-8926-478a-9c14-90cf904dd930"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Shoot"",
                    ""type"": ""Button"",
                    ""id"": ""218b1ec4-6b2d-459b-bf12-9e0a46fa8adb"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Movement"",
                    ""type"": ""Value"",
                    ""id"": ""028a1d11-8cc7-49af-b8dd-8f3a6913b0c3"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Camera"",
                    ""type"": ""PassThrough"",
                    ""id"": ""8805d499-17c3-4b33-b509-790a17f3a632"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Aim"",
                    ""type"": ""Button"",
                    ""id"": ""8bb183ea-68d2-40f4-aea5-3eb3087dc3c4"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""c5d1fba5-7ea3-4f38-bceb-34a674f98d46"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3ec119fa-09a3-4d79-9543-b5761a8c8ba4"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Slide"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""946feaf1-c4b9-46a4-b5ef-56e0d3807682"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Shoot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""3f8917c7-7d55-44a7-baaf-3cdebe8967c3"",
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
                    ""id"": ""b450b771-edad-4606-ae8c-e877c8218e91"",
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
                    ""id"": ""3b31ab1d-04ea-4b32-8292-ce972c1f5642"",
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
                    ""id"": ""ae648c9d-1c31-4d27-bd94-c9e9b2fe5483"",
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
                    ""id"": ""ae404e22-ae3a-4780-9690-f554e8644ab9"",
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
                    ""id"": ""f5e49181-4371-48b0-95cf-6ec1ab72f74e"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Camera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""fedc170e-9e87-4faa-bb5a-9ef1ef1293f3"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Aim"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Tutorial
        m_Tutorial = asset.FindActionMap("Tutorial", throwIfNotFound: true);
        m_Tutorial_Jump = m_Tutorial.FindAction("Jump", throwIfNotFound: true);
        m_Tutorial_Slide = m_Tutorial.FindAction("Slide", throwIfNotFound: true);
        m_Tutorial_Shoot = m_Tutorial.FindAction("Shoot", throwIfNotFound: true);
        m_Tutorial_Movement = m_Tutorial.FindAction("Movement", throwIfNotFound: true);
        m_Tutorial_Camera = m_Tutorial.FindAction("Camera", throwIfNotFound: true);
        m_Tutorial_Aim = m_Tutorial.FindAction("Aim", throwIfNotFound: true);
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

    // Tutorial
    private readonly InputActionMap m_Tutorial;
    private ITutorialActions m_TutorialActionsCallbackInterface;
    private readonly InputAction m_Tutorial_Jump;
    private readonly InputAction m_Tutorial_Slide;
    private readonly InputAction m_Tutorial_Shoot;
    private readonly InputAction m_Tutorial_Movement;
    private readonly InputAction m_Tutorial_Camera;
    private readonly InputAction m_Tutorial_Aim;
    public struct TutorialActions
    {
        private @PlayerActions m_Wrapper;
        public TutorialActions(@PlayerActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Jump => m_Wrapper.m_Tutorial_Jump;
        public InputAction @Slide => m_Wrapper.m_Tutorial_Slide;
        public InputAction @Shoot => m_Wrapper.m_Tutorial_Shoot;
        public InputAction @Movement => m_Wrapper.m_Tutorial_Movement;
        public InputAction @Camera => m_Wrapper.m_Tutorial_Camera;
        public InputAction @Aim => m_Wrapper.m_Tutorial_Aim;
        public InputActionMap Get() { return m_Wrapper.m_Tutorial; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(TutorialActions set) { return set.Get(); }
        public void SetCallbacks(ITutorialActions instance)
        {
            if (m_Wrapper.m_TutorialActionsCallbackInterface != null)
            {
                @Jump.started -= m_Wrapper.m_TutorialActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_TutorialActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_TutorialActionsCallbackInterface.OnJump;
                @Slide.started -= m_Wrapper.m_TutorialActionsCallbackInterface.OnSlide;
                @Slide.performed -= m_Wrapper.m_TutorialActionsCallbackInterface.OnSlide;
                @Slide.canceled -= m_Wrapper.m_TutorialActionsCallbackInterface.OnSlide;
                @Shoot.started -= m_Wrapper.m_TutorialActionsCallbackInterface.OnShoot;
                @Shoot.performed -= m_Wrapper.m_TutorialActionsCallbackInterface.OnShoot;
                @Shoot.canceled -= m_Wrapper.m_TutorialActionsCallbackInterface.OnShoot;
                @Movement.started -= m_Wrapper.m_TutorialActionsCallbackInterface.OnMovement;
                @Movement.performed -= m_Wrapper.m_TutorialActionsCallbackInterface.OnMovement;
                @Movement.canceled -= m_Wrapper.m_TutorialActionsCallbackInterface.OnMovement;
                @Camera.started -= m_Wrapper.m_TutorialActionsCallbackInterface.OnCamera;
                @Camera.performed -= m_Wrapper.m_TutorialActionsCallbackInterface.OnCamera;
                @Camera.canceled -= m_Wrapper.m_TutorialActionsCallbackInterface.OnCamera;
                @Aim.started -= m_Wrapper.m_TutorialActionsCallbackInterface.OnAim;
                @Aim.performed -= m_Wrapper.m_TutorialActionsCallbackInterface.OnAim;
                @Aim.canceled -= m_Wrapper.m_TutorialActionsCallbackInterface.OnAim;
            }
            m_Wrapper.m_TutorialActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
                @Slide.started += instance.OnSlide;
                @Slide.performed += instance.OnSlide;
                @Slide.canceled += instance.OnSlide;
                @Shoot.started += instance.OnShoot;
                @Shoot.performed += instance.OnShoot;
                @Shoot.canceled += instance.OnShoot;
                @Movement.started += instance.OnMovement;
                @Movement.performed += instance.OnMovement;
                @Movement.canceled += instance.OnMovement;
                @Camera.started += instance.OnCamera;
                @Camera.performed += instance.OnCamera;
                @Camera.canceled += instance.OnCamera;
                @Aim.started += instance.OnAim;
                @Aim.performed += instance.OnAim;
                @Aim.canceled += instance.OnAim;
            }
        }
    }
    public TutorialActions @Tutorial => new TutorialActions(this);
    public interface ITutorialActions
    {
        void OnJump(InputAction.CallbackContext context);
        void OnSlide(InputAction.CallbackContext context);
        void OnShoot(InputAction.CallbackContext context);
        void OnMovement(InputAction.CallbackContext context);
        void OnCamera(InputAction.CallbackContext context);
        void OnAim(InputAction.CallbackContext context);
    }
}
