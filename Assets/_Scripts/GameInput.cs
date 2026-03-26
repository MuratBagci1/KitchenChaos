using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    private const string PLAYER_PREFS_BINDING = "InputBindings";

    public enum Binding
    {
        Move_Up,
        Move_Down,
        Move_Left,
        Move_Right,
        Interact,
        Interact_Alternate,
        Pause,
        Gamepad_Interact,
        Gamepad_Interact_Alternate,
        Gamepad_Pause
    }

    private PlayerInput playerInput;

    public event EventHandler OnInteractAction;

    public event EventHandler OnInteractAlternateAction;

    public event EventHandler OnPauseAction;

    public event EventHandler OnBindingRebind;

    public static GameInput Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

        playerInput = new PlayerInput(); 
        
        if (PlayerPrefs.HasKey(PLAYER_PREFS_BINDING))
        {
            string outputJSON = PlayerPrefs.GetString(PLAYER_PREFS_BINDING);
            playerInput.LoadBindingOverridesFromJson(outputJSON);
        }

        playerInput.Player.Enable();

        playerInput.Player.Interact.performed += Interacted;
        playerInput.Player.InteractAlternate.performed += InteractedAlternate;
        playerInput.Player.Pause.performed += Pause_performed;        
    }

    private void OnDestroy()
    {
        playerInput.Player.Interact.performed -= Interacted;
        playerInput.Player.InteractAlternate.performed -= InteractedAlternate;
        playerInput.Player.Pause.performed -= Pause_performed;

        playerInput.Dispose();
    }

    private void Pause_performed(InputAction.CallbackContext obj)
    {
        OnPauseAction?.Invoke(this, EventArgs.Empty);
    }

    private void Interacted(InputAction.CallbackContext obj)
    {
        OnInteractAction?.Invoke(this, EventArgs.Empty);
    }

    private void InteractedAlternate(InputAction.CallbackContext obj)
    {
        OnInteractAlternateAction?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 GetMovementVectorNormalized()
    {
        Vector2 inputVector = playerInput.Player.Move.ReadValue<Vector2>();

        //processor ile de normalized edilebilir
        inputVector = inputVector.normalized;

        return inputVector;
    }

    public string GetBindingText(Binding binding)
    {
        switch (binding)
        {
            case Binding.Move_Up:
                return playerInput.Player.Move.bindings[1].ToDisplayString();
            case Binding.Move_Down:
                return playerInput.Player.Move.bindings[2].ToDisplayString();
            case Binding.Move_Left:
                return playerInput.Player.Move.bindings[3].ToDisplayString();
            case Binding.Move_Right:
                return playerInput.Player.Move.bindings[4].ToDisplayString();
            case Binding.Interact:
                return playerInput.Player.Interact.bindings[0].ToDisplayString();
            case Binding.Interact_Alternate:
                return playerInput.Player.InteractAlternate.bindings[0].ToDisplayString();
            case Binding.Pause:
                return playerInput.Player.Pause.bindings[0].ToDisplayString();
            case Binding.Gamepad_Interact:
                return playerInput.Player.Interact.bindings[1].ToDisplayString();
            case Binding.Gamepad_Interact_Alternate:
                return playerInput.Player.InteractAlternate.bindings[1].ToDisplayString();
            case Binding.Gamepad_Pause:
                return playerInput.Player.Pause.bindings[1].ToDisplayString();
            default:
                return null;
        }
    }

    public void RebindBinding(Binding binding, Action onActionRebound)
    {
        InputAction action;
        int bindingIndex;

        switch (binding)
        {
            case Binding.Move_Up:
                action = playerInput.Player.Move;
                bindingIndex = 1;
                break;
            case Binding.Move_Down:
                action = playerInput.Player.Move;
                bindingIndex = 2;
                break;
            case Binding.Move_Left:
                action = playerInput.Player.Move;
                bindingIndex = 3;
                break;
            case Binding.Move_Right:
                action = playerInput.Player.Move;
                bindingIndex = 4;
                break;
            case Binding.Interact:
                action = playerInput.Player.Interact;
                bindingIndex = 0;
                break;
            case Binding.Interact_Alternate:
                action = playerInput.Player.InteractAlternate;
                bindingIndex = 0;
                break;
            case Binding.Pause:
                action = playerInput.Player.Pause;
                bindingIndex = 0;
                break;
            case Binding.Gamepad_Interact:
                action = playerInput.Player.Interact;
                bindingIndex = 1;
                break;
            case Binding.Gamepad_Interact_Alternate:
                action = playerInput.Player.InteractAlternate;
                bindingIndex = 1;
                break;
            case Binding.Gamepad_Pause:
                action = playerInput.Player.Pause;
                bindingIndex = 1;
                break;
            default:
                action = null;
                bindingIndex = 0;
                break;
        }

        action.Disable();

        action.PerformInteractiveRebinding(bindingIndex).WithCancelingThrough("<Keyboard>/escape") // optional: allow canceling
            .OnCancel(callback =>
            {
                callback.Dispose();
                action.Enable();
                onActionRebound?.Invoke();
            })
            .OnComplete(callback =>
            {
                callback.Dispose();
                action.Enable();    
                onActionRebound?.Invoke();

                string inputJSON = playerInput.SaveBindingOverridesAsJson();
                PlayerPrefs.SetString(PLAYER_PREFS_BINDING, inputJSON);
                PlayerPrefs.Save();

                OnBindingRebind?.Invoke(this, EventArgs.Empty);
            })
            .Start();
    }
}