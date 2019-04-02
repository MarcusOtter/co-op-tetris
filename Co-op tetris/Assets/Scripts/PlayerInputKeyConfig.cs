using UnityEngine;

[CreateAssetMenu(menuName ="Input config")]
public class PlayerInputKeyConfig : ScriptableObject
{
    [Header("Player number")]
    public int PlayerNumber = 1;

    [Header("Axes")]
    [SerializeField] private string _verticalAxis = "L_Vertical";
    [SerializeField] private string _horizontalAxis = "L_Horizontal";

    [Header("Action keys")]
    [SerializeField] private string _actionKey1 = "L_Btn1";
    [SerializeField] private string _actionKey2 = "L_Btn2";
    [SerializeField] private string _actionKey3 = "L_Btn3";

    internal string GetAxisNameFor(PlayerInputAction action)
    {
        switch (action)
        {
            case PlayerInputAction.Up:
            case PlayerInputAction.Down:
                return _verticalAxis;

            case PlayerInputAction.Left:
            case PlayerInputAction.Right:
                return _horizontalAxis;

            default: throw new System.Exception($"'{action}'does not have an axis.");
        }
    }

    internal string GetButtonNameFor(PlayerInputAction action)
    {
        switch (action)
        {
            case PlayerInputAction.Action1: return _actionKey1;
            case PlayerInputAction.Action2: return _actionKey2;
            case PlayerInputAction.Action3: return _actionKey3;

            default: throw new System.Exception($"'{action}' does not have a button.");
        }
    }
}
