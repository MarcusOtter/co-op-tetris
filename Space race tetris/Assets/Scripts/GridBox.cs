using UnityEngine;

public class GridBox
{
    internal readonly Vector2Int Position;
    internal bool IsOccupied;

    public GridBox(Vector2Int position)
    {
        Position = position;
    }
}
