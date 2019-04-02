using System;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    internal static event EventHandler<int> OnUpDirectionPressed;
    internal static event EventHandler<int> OnLeftDirectionPressed;
    internal static event EventHandler<int> OnDownDirectionPressed;
    internal static event EventHandler<int> OnDownDirectionReleased;
    internal static event EventHandler<int> OnRightDirectionPressed;

    internal static event EventHandler<int> OnAction1ButtonPressed;
    internal static event EventHandler<int> OnAction2ButtonPressed;
    internal static event EventHandler<int> OnAction3ButtonPressed;

    [Header("Input configurations")]
    [SerializeField] private PlayerInputKeyConfig[] _inputConfigs;

    [Header("Other input settings")]
    [SerializeField] private float _joystickDeadzone = 0.1f;

    private List<int> _playersPressingDownDirection = new List<int>();

    private void OnEnable()
    {
        OnDownDirectionPressed += RegisterPlayerPressingDownDirection;
    }

    private void Update()
    {
        CheckForInputEvents();
    }

    private void CheckForInputEvents()
    {
        foreach(var config in _inputConfigs)
        {
            // Action buttons
            if (ActionButtonPressed(PlayerInputAction.Action1, config)) { OnAction1ButtonPressed?.Invoke(this, config.PlayerNumber); }
            if (ActionButtonPressed(PlayerInputAction.Action2, config)) { OnAction2ButtonPressed?.Invoke(this, config.PlayerNumber); }
            if (ActionButtonPressed(PlayerInputAction.Action3, config)) { OnAction3ButtonPressed?.Invoke(this, config.PlayerNumber); }



            // These axis need to be changed... 
            // Called when the axis is more than the deadzone
            // not when it's newly over the deadzone

            // (check if the axis is already pressed in that direction.. somehow)

            // Horizontal axis
            if      (AxisDirectionPressed(PlayerInputAction.Right, config)) { OnRightDirectionPressed?.Invoke(this, config.PlayerNumber); }
            else if (AxisDirectionPressed(PlayerInputAction.Left,  config)) { OnLeftDirectionPressed ?.Invoke(this, config.PlayerNumber); }

            // Vertical axis
            if      (AxisDirectionPressed(PlayerInputAction.Up, config))   { OnUpDirectionPressed  ?.Invoke(this, config.PlayerNumber); }
            else if (AxisDirectionPressed(PlayerInputAction.Down, config)) { OnDownDirectionPressed?.Invoke(this, config.PlayerNumber); }

            // If this player was pressing the joystick downwards but now stopped doing so
            else if (_playersPressingDownDirection.Contains(config.PlayerNumber)) 
            {
                _playersPressingDownDirection.Remove(config.PlayerNumber);
                OnDownDirectionReleased?.Invoke(this, config.PlayerNumber);
            }
        }
    }

    private bool AxisDirectionPressed(PlayerInputAction action, PlayerInputKeyConfig config)
    {
        float axisValue = Input.GetAxisRaw(config.GetAxisNameFor(action));

        switch (action)
        {
            case PlayerInputAction.Up:
            case PlayerInputAction.Right:
                return axisValue > _joystickDeadzone;

            case PlayerInputAction.Down:
            case PlayerInputAction.Left:
                return axisValue < -_joystickDeadzone;

            default: throw new Exception($"{action} does not have an axis.");
        }
    }

    private bool ActionButtonPressed(PlayerInputAction action, PlayerInputKeyConfig config)
    {
        return Input.GetButtonDown(config.GetButtonNameFor(action));
    }

    private void RegisterPlayerPressingDownDirection(object sender, int playerNumber)
    {
        if (_playersPressingDownDirection.Contains(playerNumber)) { return;  }
        _playersPressingDownDirection.Add(playerNumber);
    }

    private void OnDisable()
    {
        OnDownDirectionPressed -= RegisterPlayerPressingDownDirection;
    }
}
