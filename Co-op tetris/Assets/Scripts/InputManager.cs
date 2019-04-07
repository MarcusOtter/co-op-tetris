using System;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    internal static int TetrominoPlayerNumber { get; private set; } = 1;

    internal static event EventHandler<PlayerInputEventArgs> OnPlayerInput;

    [Header("Input configurations")]
    [SerializeField] private PlayerInputKeyConfig[] _inputConfigs;

    [Header("Other input settings")]
    [SerializeField] private float _joystickDeadzone = 0.1f;

    private Dictionary<(PlayerInputKeyConfig, PlayerInputAction), bool> _playerIsPressingInputAction 
        = new Dictionary<(PlayerInputKeyConfig, PlayerInputAction), bool>();

    private PlayerInputAction[] _availableInputActions;

    private void Awake()
    {
        _availableInputActions = (PlayerInputAction[]) Enum.GetValues(typeof(PlayerInputAction));

        // Initialize dictionary
        foreach (var inputConfig in _inputConfigs)
        {
            foreach (var inputAction in _availableInputActions)
            {
                _playerIsPressingInputAction[(inputConfig, inputAction)] = false;
            }
        }
    }

    private void Update()
    {
        foreach (var config in _inputConfigs)
        {
            foreach (var action in _availableInputActions)
            {
                _playerIsPressingInputAction[(config, action)] = InputActionBeingPressed(config, action);
            }
        }
    }

    // Doesn't support more than 2 players (like the rest of this class)
    internal static void SwapTetrominoPlayer()
    {
        TetrominoPlayerNumber = TetrominoPlayerNumber == 1 ? 2 : 1;
    }

    private bool InputActionBeingPressed(PlayerInputKeyConfig config, PlayerInputAction action)
    {
        bool wasPressedLastFrame = _playerIsPressingInputAction[(config, action)];
        bool beingPressed = false;

        switch (action)
        {
            case PlayerInputAction.Action1:
            case PlayerInputAction.Action2:
            case PlayerInputAction.Action3:
                beingPressed = Input.GetButton(config.GetButtonNameFor(action));
                break;

            case PlayerInputAction.Up:
            case PlayerInputAction.Down:
                float verticalAxisValue = Input.GetAxisRaw(config.GetAxisNameFor(action));
                beingPressed = action == PlayerInputAction.Up
                    ? verticalAxisValue > _joystickDeadzone
                    : verticalAxisValue < -_joystickDeadzone;
                break;

            case PlayerInputAction.Right:
            case PlayerInputAction.Left:
                float horizontalAxisValue = Input.GetAxisRaw(config.GetAxisNameFor(action));
                beingPressed = action == PlayerInputAction.Right
                    ? horizontalAxisValue > _joystickDeadzone
                    : horizontalAxisValue < -_joystickDeadzone;
                break;
        }

        if (!wasPressedLastFrame && beingPressed)
        {
            OnPlayerInput?.Invoke(this, new PlayerInputEventArgs(config.PlayerNumber, InputState.Pressed, action));
        }
        else if (wasPressedLastFrame && !beingPressed)
        {
            OnPlayerInput?.Invoke(this, new PlayerInputEventArgs(config.PlayerNumber, InputState.Released, action));
        }

        return beingPressed;
    }
}
