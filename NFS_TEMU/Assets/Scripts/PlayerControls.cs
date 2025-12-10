










using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;


























































public partial class @PlayerControls: IInputActionCollection2, IDisposable
{
    
    
    
    public InputActionAsset asset { get; }

    
    
    
    public @PlayerControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""version"": 1,
    ""name"": ""PlayerControls"",
    ""maps"": [
        {
            ""name"": ""Race"",
            ""id"": ""9c0d25dd-190b-49be-9e14-623311bcb114"",
            ""actions"": [
                {
                    ""name"": ""Girar"",
                    ""type"": ""Value"",
                    ""id"": ""4cdf6115-360b-4d55-ae71-e213ead3d737"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Acelerar"",
                    ""type"": ""Value"",
                    ""id"": ""97a55391-7098-477b-b41e-066c2917488c"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Frenar"",
                    ""type"": ""Value"",
                    ""id"": ""727b04fc-8975-4d0e-9258-7a875201a331"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""f5de857f-eeed-4256-8ea6-3e159bc1459b"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Girar"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""e776eaf5-74ea-44ab-b2a7-7c7b2aaec1fa"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Girar"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""e2050f38-49ed-49a3-bb8c-a935abac7d41"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Girar"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""1ab99b58-8812-47ae-9423-abb78d62e1ee"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Acelerar"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e4d0cd25-0aca-44c9-8748-23f29a70c5f2"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Acelerar"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""177307f4-0b1f-4513-91d8-c0b47c08ffb2"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Frenar"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f208ab12-8f54-44ac-b241-69a985bc110f"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Frenar"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        
        m_Race = asset.FindActionMap("Race", throwIfNotFound: true);
        m_Race_Girar = m_Race.FindAction("Girar", throwIfNotFound: true);
        m_Race_Acelerar = m_Race.FindAction("Acelerar", throwIfNotFound: true);
        m_Race_Frenar = m_Race.FindAction("Frenar", throwIfNotFound: true);
    }

    ~@PlayerControls()
    {
        UnityEngine.Debug.Assert(!m_Race.enabled, "This will cause a leak and performance issues, PlayerControls.Race.Disable() has not been called.");
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

    
    private readonly InputActionMap m_Race;
    private List<IRaceActions> m_RaceActionsCallbackInterfaces = new List<IRaceActions>();
    private readonly InputAction m_Race_Girar;
    private readonly InputAction m_Race_Acelerar;
    private readonly InputAction m_Race_Frenar;
    
    
    
    public struct RaceActions
    {
        private @PlayerControls m_Wrapper;

        
        
        
        public RaceActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        
        
        
        public InputAction @Girar => m_Wrapper.m_Race_Girar;
        
        
        
        public InputAction @Acelerar => m_Wrapper.m_Race_Acelerar;
        
        
        
        public InputAction @Frenar => m_Wrapper.m_Race_Frenar;
        
        
        
        public InputActionMap Get() { return m_Wrapper.m_Race; }
        
        public void Enable() { Get().Enable(); }
        
        public void Disable() { Get().Disable(); }
        
        public bool enabled => Get().enabled;
        
        
        
        public static implicit operator InputActionMap(RaceActions set) { return set.Get(); }
        
        
        
        
        
        
        
        
        public void AddCallbacks(IRaceActions instance)
        {
            if (instance == null || m_Wrapper.m_RaceActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_RaceActionsCallbackInterfaces.Add(instance);
            @Girar.started += instance.OnGirar;
            @Girar.performed += instance.OnGirar;
            @Girar.canceled += instance.OnGirar;
            @Acelerar.started += instance.OnAcelerar;
            @Acelerar.performed += instance.OnAcelerar;
            @Acelerar.canceled += instance.OnAcelerar;
            @Frenar.started += instance.OnFrenar;
            @Frenar.performed += instance.OnFrenar;
            @Frenar.canceled += instance.OnFrenar;
        }

        
        
        
        
        
        
        
        private void UnregisterCallbacks(IRaceActions instance)
        {
            @Girar.started -= instance.OnGirar;
            @Girar.performed -= instance.OnGirar;
            @Girar.canceled -= instance.OnGirar;
            @Acelerar.started -= instance.OnAcelerar;
            @Acelerar.performed -= instance.OnAcelerar;
            @Acelerar.canceled -= instance.OnAcelerar;
            @Frenar.started -= instance.OnFrenar;
            @Frenar.performed -= instance.OnFrenar;
            @Frenar.canceled -= instance.OnFrenar;
        }

        
        
        
        
        public void RemoveCallbacks(IRaceActions instance)
        {
            if (m_Wrapper.m_RaceActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        
        
        
        
        
        
        
        
        
        public void SetCallbacks(IRaceActions instance)
        {
            foreach (var item in m_Wrapper.m_RaceActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_RaceActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    
    
    
    public RaceActions @Race => new RaceActions(this);
    
    
    
    
    
    public interface IRaceActions
    {
        
        
        
        
        
        
        void OnGirar(InputAction.CallbackContext context);
        
        
        
        
        
        
        void OnAcelerar(InputAction.CallbackContext context);
        
        
        
        
        
        
        void OnFrenar(InputAction.CallbackContext context);
    }
}
