using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    // TODO: 2 player input
    // have an enum with different input modes i guess
    internal static event EventHandler OnDownKeyDown;
    internal static event EventHandler OnDownKeyUp;

    internal static event EventHandler OnUpKeyDown;
    internal static event EventHandler OnUpKeyUp;

    internal static event EventHandler OnLeftKeyDown;
    internal static event EventHandler OnLeftKeyUp;

    internal static event EventHandler OnRightKeyDown;
    internal static event EventHandler OnRightKeyUp;

    private void Update()
    {
        CheckForInputEvents();
    }

    private void CheckForInputEvents()
    {
        if      (Input.GetKeyDown(KeyCode.DownArrow))  { OnDownKeyDown? .Invoke(this, EventArgs.Empty); }
        else if (Input.GetKeyUp  (KeyCode.DownArrow))  { OnDownKeyUp?   .Invoke(this, EventArgs.Empty); }

        if      (Input.GetKeyDown(KeyCode.UpArrow))    { OnUpKeyDown?   .Invoke(this, EventArgs.Empty); }
        else if (Input.GetKeyUp  (KeyCode.UpArrow))    { OnUpKeyUp?     .Invoke(this, EventArgs.Empty); }

        if      (Input.GetKeyDown(KeyCode.LeftArrow))  { OnLeftKeyDown? .Invoke(this, EventArgs.Empty); }
        else if (Input.GetKeyUp  (KeyCode.LeftArrow))  { OnLeftKeyUp?   .Invoke(this, EventArgs.Empty); }

        if      (Input.GetKeyDown(KeyCode.RightArrow)) { OnRightKeyDown?.Invoke(this, EventArgs.Empty); }
        else if (Input.GetKeyUp  (KeyCode.RightArrow)) { OnRightKeyUp?  .Invoke(this, EventArgs.Empty); }
    }
}
