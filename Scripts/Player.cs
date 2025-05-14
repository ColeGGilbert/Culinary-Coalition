// GENERATED AUTOMATICALLY FROM 'Assets/Scripts/Player.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @Player : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @Player()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Player"",
    ""maps"": [
        {
            ""name"": ""Gameplay"",
            ""id"": ""a7dee02d-cd1c-4a39-8853-382f4893fcf1"",
            ""actions"": [
                {
                    ""name"": ""Dash"",
                    ""type"": ""Button"",
                    ""id"": ""1edd090b-de23-4164-a8e5-8dfd5cb7b4a8"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Skip Dialogue"",
                    ""type"": ""Button"",
                    ""id"": ""3d5f83b6-63a9-419b-960d-7e291dc92a42"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Pausing"",
                    ""type"": ""Button"",
                    ""id"": ""c3cf8c4a-e600-4075-be33-07344e156ae4"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Interaction"",
                    ""type"": ""Button"",
                    ""id"": ""e6582f1d-8443-4e76-b37e-8f0525c16f10"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Hatcuum"",
                    ""type"": ""Button"",
                    ""id"": ""56a08635-a2d2-44d5-ade0-626ade9c76b4"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Eating"",
                    ""type"": ""Button"",
                    ""id"": ""8e59c113-e72c-4b39-96c7-fc3c3497d4a2"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Movement"",
                    ""type"": ""Button"",
                    ""id"": ""37aa0db5-666e-4ba2-a033-0bb3eb2e5c03"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ServeEarly"",
                    ""type"": ""Button"",
                    ""id"": ""532395b2-8562-427d-b424-be2699d11b51"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Any"",
                    ""type"": ""Button"",
                    ""id"": ""fa3ca71c-929e-46ae-9cda-ed26c740e7d7"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Up"",
                    ""type"": ""Button"",
                    ""id"": ""ea6839bc-179f-400f-b555-9eeab67973a6"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Down"",
                    ""type"": ""Button"",
                    ""id"": ""19a55140-0a52-4bb9-a4a8-c56782da97fc"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Left"",
                    ""type"": ""Button"",
                    ""id"": ""ab7b21d9-0897-4852-923f-8bd936dc0a3d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Right"",
                    ""type"": ""Button"",
                    ""id"": ""284b7713-646b-4f34-9dbc-e4feefbd569e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Select"",
                    ""type"": ""Button"",
                    ""id"": ""dfaf596b-5cb3-48ca-8b1d-eaa3278f9825"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ResetFailsafeL"",
                    ""type"": ""Button"",
                    ""id"": ""91680da5-9c84-4f2f-a80c-8d309e6cc561"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Hold(duration=3)""
                },
                {
                    ""name"": ""ResetFailsafeR"",
                    ""type"": ""Button"",
                    ""id"": ""8251b700-cca9-47b3-bdc4-59ba72a58e7c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Hold(duration=3)""
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""757efb2c-957d-4f1d-a936-32bb3f478b89"",
                    ""path"": ""<Gamepad>/leftTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Dash"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""fe69902e-6292-406c-ace5-220689815a19"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Dash"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d1fd3460-dc7a-416a-ad9b-ca51aa6624c3"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Skip Dialogue"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4a26f547-8864-47c3-9d3a-f9f47bbb4e43"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Pausing"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""296fb72b-80b4-4a18-96ac-cdc71d26be28"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Interaction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7b6f9f82-3079-4ea5-bc70-c268f82b02ef"",
                    ""path"": ""<Gamepad>/buttonNorth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Hatcuum"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ecc21c43-6d1e-4cd0-97a8-2cfba867209f"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Eating"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c00046b4-6f1e-4bd4-90ea-bddc438b35f1"",
                    ""path"": ""<Gamepad>/leftStick/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""315b0bff-052b-4bce-86fd-44611ba6c2ef"",
                    ""path"": ""<Gamepad>/leftStick/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8b009f29-b2fe-44da-b5b4-9599adecf981"",
                    ""path"": ""<Gamepad>/leftStick/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6b4adc49-56fd-4947-8ef4-645f579d8431"",
                    ""path"": ""<Gamepad>/leftStick/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a1fa605a-76d3-4c7c-ad4e-1cb8ba24a44c"",
                    ""path"": ""<Gamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ServeEarly"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c7e37dd7-c89b-47a0-b640-738b90aac198"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Any"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8ff6799f-3be3-4dd4-afd0-17541c2547d4"",
                    ""path"": ""<Gamepad>/buttonNorth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Any"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9ec216eb-51e0-4596-83b1-9225858d36c0"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Any"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e0774409-232d-41a9-9dae-673469aa97c5"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Any"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b02d34c2-6271-4a62-8f09-182a11ea4b6c"",
                    ""path"": ""<Gamepad>/dpad/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Any"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b1cad40d-0ee4-4953-bdc1-62af438bc1cb"",
                    ""path"": ""<Gamepad>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Any"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6a287837-bdde-4976-830c-0c57ba842bbd"",
                    ""path"": ""<Gamepad>/leftStickPress"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Any"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5d4aeeae-f7a6-451c-9704-a8e8cc1aabd4"",
                    ""path"": ""<Gamepad>/leftTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Any"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e564bdd7-d697-4f67-9432-3e4d182e14f9"",
                    ""path"": ""<Gamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Any"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""52152223-9c35-4313-9a03-0d98f8b9a5fa"",
                    ""path"": ""<Gamepad>/rightStickPress"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Any"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""95166f8f-6f4c-4659-97e4-e06185a9ba64"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Any"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""cc30ebb9-386f-4864-b4a7-38a86b960573"",
                    ""path"": ""<Gamepad>/select"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Any"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""669b8ff8-81a0-4815-a52b-69273e042d86"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Any"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""841ef32f-7f6d-4dcf-9054-cfa66ead5a8a"",
                    ""path"": ""<Gamepad>/dpad/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Any"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5dde98e1-72a3-4f16-9bcb-f860f32e5c8d"",
                    ""path"": ""<Gamepad>/dpad/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Any"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""41c6f266-e2f8-41e1-98c2-41ed5af92da3"",
                    ""path"": ""<Gamepad>/dpad/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Any"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""abbcf041-791b-4381-aada-d96d5583bf6e"",
                    ""path"": ""<Gamepad>/dpad/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Up"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a9e73152-74c6-4b18-98d7-65b0f7a49468"",
                    ""path"": ""<Gamepad>/dpad/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Down"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9bb9131a-1021-4a19-a0c2-1182a934fa8f"",
                    ""path"": ""<Gamepad>/dpad/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Left"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7b298e42-ae99-485f-94c7-7bbce999808b"",
                    ""path"": ""<Gamepad>/dpad/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Right"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a19047ef-d97b-4025-9906-7be600868962"",
                    ""path"": ""<Gamepad>/select"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Select"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c801c5bb-a1eb-4b2c-ad56-35403a5f338a"",
                    ""path"": ""<Gamepad>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ResetFailsafeL"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3b6b4d31-7b07-4313-a4ce-03583af5a1f6"",
                    ""path"": ""<Gamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ResetFailsafeR"",
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
        m_Gameplay_Dash = m_Gameplay.FindAction("Dash", throwIfNotFound: true);
        m_Gameplay_SkipDialogue = m_Gameplay.FindAction("Skip Dialogue", throwIfNotFound: true);
        m_Gameplay_Pausing = m_Gameplay.FindAction("Pausing", throwIfNotFound: true);
        m_Gameplay_Interaction = m_Gameplay.FindAction("Interaction", throwIfNotFound: true);
        m_Gameplay_Hatcuum = m_Gameplay.FindAction("Hatcuum", throwIfNotFound: true);
        m_Gameplay_Eating = m_Gameplay.FindAction("Eating", throwIfNotFound: true);
        m_Gameplay_Movement = m_Gameplay.FindAction("Movement", throwIfNotFound: true);
        m_Gameplay_ServeEarly = m_Gameplay.FindAction("ServeEarly", throwIfNotFound: true);
        m_Gameplay_Any = m_Gameplay.FindAction("Any", throwIfNotFound: true);
        m_Gameplay_Up = m_Gameplay.FindAction("Up", throwIfNotFound: true);
        m_Gameplay_Down = m_Gameplay.FindAction("Down", throwIfNotFound: true);
        m_Gameplay_Left = m_Gameplay.FindAction("Left", throwIfNotFound: true);
        m_Gameplay_Right = m_Gameplay.FindAction("Right", throwIfNotFound: true);
        m_Gameplay_Select = m_Gameplay.FindAction("Select", throwIfNotFound: true);
        m_Gameplay_ResetFailsafeL = m_Gameplay.FindAction("ResetFailsafeL", throwIfNotFound: true);
        m_Gameplay_ResetFailsafeR = m_Gameplay.FindAction("ResetFailsafeR", throwIfNotFound: true);
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

    // Gameplay
    private readonly InputActionMap m_Gameplay;
    private IGameplayActions m_GameplayActionsCallbackInterface;
    private readonly InputAction m_Gameplay_Dash;
    private readonly InputAction m_Gameplay_SkipDialogue;
    private readonly InputAction m_Gameplay_Pausing;
    private readonly InputAction m_Gameplay_Interaction;
    private readonly InputAction m_Gameplay_Hatcuum;
    private readonly InputAction m_Gameplay_Eating;
    private readonly InputAction m_Gameplay_Movement;
    private readonly InputAction m_Gameplay_ServeEarly;
    private readonly InputAction m_Gameplay_Any;
    private readonly InputAction m_Gameplay_Up;
    private readonly InputAction m_Gameplay_Down;
    private readonly InputAction m_Gameplay_Left;
    private readonly InputAction m_Gameplay_Right;
    private readonly InputAction m_Gameplay_Select;
    private readonly InputAction m_Gameplay_ResetFailsafeL;
    private readonly InputAction m_Gameplay_ResetFailsafeR;
    public struct GameplayActions
    {
        private @Player m_Wrapper;
        public GameplayActions(@Player wrapper) { m_Wrapper = wrapper; }
        public InputAction @Dash => m_Wrapper.m_Gameplay_Dash;
        public InputAction @SkipDialogue => m_Wrapper.m_Gameplay_SkipDialogue;
        public InputAction @Pausing => m_Wrapper.m_Gameplay_Pausing;
        public InputAction @Interaction => m_Wrapper.m_Gameplay_Interaction;
        public InputAction @Hatcuum => m_Wrapper.m_Gameplay_Hatcuum;
        public InputAction @Eating => m_Wrapper.m_Gameplay_Eating;
        public InputAction @Movement => m_Wrapper.m_Gameplay_Movement;
        public InputAction @ServeEarly => m_Wrapper.m_Gameplay_ServeEarly;
        public InputAction @Any => m_Wrapper.m_Gameplay_Any;
        public InputAction @Up => m_Wrapper.m_Gameplay_Up;
        public InputAction @Down => m_Wrapper.m_Gameplay_Down;
        public InputAction @Left => m_Wrapper.m_Gameplay_Left;
        public InputAction @Right => m_Wrapper.m_Gameplay_Right;
        public InputAction @Select => m_Wrapper.m_Gameplay_Select;
        public InputAction @ResetFailsafeL => m_Wrapper.m_Gameplay_ResetFailsafeL;
        public InputAction @ResetFailsafeR => m_Wrapper.m_Gameplay_ResetFailsafeR;
        public InputActionMap Get() { return m_Wrapper.m_Gameplay; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(GameplayActions set) { return set.Get(); }
        public void SetCallbacks(IGameplayActions instance)
        {
            if (m_Wrapper.m_GameplayActionsCallbackInterface != null)
            {
                @Dash.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnDash;
                @Dash.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnDash;
                @Dash.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnDash;
                @SkipDialogue.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnSkipDialogue;
                @SkipDialogue.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnSkipDialogue;
                @SkipDialogue.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnSkipDialogue;
                @Pausing.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnPausing;
                @Pausing.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnPausing;
                @Pausing.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnPausing;
                @Interaction.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnInteraction;
                @Interaction.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnInteraction;
                @Interaction.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnInteraction;
                @Hatcuum.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnHatcuum;
                @Hatcuum.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnHatcuum;
                @Hatcuum.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnHatcuum;
                @Eating.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnEating;
                @Eating.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnEating;
                @Eating.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnEating;
                @Movement.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMovement;
                @Movement.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMovement;
                @Movement.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMovement;
                @ServeEarly.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnServeEarly;
                @ServeEarly.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnServeEarly;
                @ServeEarly.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnServeEarly;
                @Any.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnAny;
                @Any.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnAny;
                @Any.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnAny;
                @Up.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnUp;
                @Up.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnUp;
                @Up.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnUp;
                @Down.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnDown;
                @Down.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnDown;
                @Down.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnDown;
                @Left.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnLeft;
                @Left.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnLeft;
                @Left.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnLeft;
                @Right.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnRight;
                @Right.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnRight;
                @Right.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnRight;
                @Select.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnSelect;
                @Select.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnSelect;
                @Select.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnSelect;
                @ResetFailsafeL.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnResetFailsafeL;
                @ResetFailsafeL.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnResetFailsafeL;
                @ResetFailsafeL.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnResetFailsafeL;
                @ResetFailsafeR.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnResetFailsafeR;
                @ResetFailsafeR.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnResetFailsafeR;
                @ResetFailsafeR.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnResetFailsafeR;
            }
            m_Wrapper.m_GameplayActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Dash.started += instance.OnDash;
                @Dash.performed += instance.OnDash;
                @Dash.canceled += instance.OnDash;
                @SkipDialogue.started += instance.OnSkipDialogue;
                @SkipDialogue.performed += instance.OnSkipDialogue;
                @SkipDialogue.canceled += instance.OnSkipDialogue;
                @Pausing.started += instance.OnPausing;
                @Pausing.performed += instance.OnPausing;
                @Pausing.canceled += instance.OnPausing;
                @Interaction.started += instance.OnInteraction;
                @Interaction.performed += instance.OnInteraction;
                @Interaction.canceled += instance.OnInteraction;
                @Hatcuum.started += instance.OnHatcuum;
                @Hatcuum.performed += instance.OnHatcuum;
                @Hatcuum.canceled += instance.OnHatcuum;
                @Eating.started += instance.OnEating;
                @Eating.performed += instance.OnEating;
                @Eating.canceled += instance.OnEating;
                @Movement.started += instance.OnMovement;
                @Movement.performed += instance.OnMovement;
                @Movement.canceled += instance.OnMovement;
                @ServeEarly.started += instance.OnServeEarly;
                @ServeEarly.performed += instance.OnServeEarly;
                @ServeEarly.canceled += instance.OnServeEarly;
                @Any.started += instance.OnAny;
                @Any.performed += instance.OnAny;
                @Any.canceled += instance.OnAny;
                @Up.started += instance.OnUp;
                @Up.performed += instance.OnUp;
                @Up.canceled += instance.OnUp;
                @Down.started += instance.OnDown;
                @Down.performed += instance.OnDown;
                @Down.canceled += instance.OnDown;
                @Left.started += instance.OnLeft;
                @Left.performed += instance.OnLeft;
                @Left.canceled += instance.OnLeft;
                @Right.started += instance.OnRight;
                @Right.performed += instance.OnRight;
                @Right.canceled += instance.OnRight;
                @Select.started += instance.OnSelect;
                @Select.performed += instance.OnSelect;
                @Select.canceled += instance.OnSelect;
                @ResetFailsafeL.started += instance.OnResetFailsafeL;
                @ResetFailsafeL.performed += instance.OnResetFailsafeL;
                @ResetFailsafeL.canceled += instance.OnResetFailsafeL;
                @ResetFailsafeR.started += instance.OnResetFailsafeR;
                @ResetFailsafeR.performed += instance.OnResetFailsafeR;
                @ResetFailsafeR.canceled += instance.OnResetFailsafeR;
            }
        }
    }
    public GameplayActions @Gameplay => new GameplayActions(this);
    public interface IGameplayActions
    {
        void OnDash(InputAction.CallbackContext context);
        void OnSkipDialogue(InputAction.CallbackContext context);
        void OnPausing(InputAction.CallbackContext context);
        void OnInteraction(InputAction.CallbackContext context);
        void OnHatcuum(InputAction.CallbackContext context);
        void OnEating(InputAction.CallbackContext context);
        void OnMovement(InputAction.CallbackContext context);
        void OnServeEarly(InputAction.CallbackContext context);
        void OnAny(InputAction.CallbackContext context);
        void OnUp(InputAction.CallbackContext context);
        void OnDown(InputAction.CallbackContext context);
        void OnLeft(InputAction.CallbackContext context);
        void OnRight(InputAction.CallbackContext context);
        void OnSelect(InputAction.CallbackContext context);
        void OnResetFailsafeL(InputAction.CallbackContext context);
        void OnResetFailsafeR(InputAction.CallbackContext context);
    }
}
