using System;

public class PlayerInputEventArgs : EventArgs
{
    internal readonly int PlayerNumber;
    internal readonly InputState InputState;
    internal readonly PlayerInputAction InputAction;

    public PlayerInputEventArgs(int playerNumber, InputState inputState, PlayerInputAction inputAction)
    {
        PlayerNumber = playerNumber;
        InputState = inputState;
        InputAction = inputAction;
    }
}
