using System.Linq;
using UnityEngine;

public class Tetromino : MonoBehaviour
{
    internal bool IsHighlighted { get; private set; }
    internal int BoxAmount => _allBoxes.Length;

    private TetrominoShape _tetrominoShape;
    private GameBoard _gameBoard;

    private Box[] _bottomBoxes;
    private Box[] _leftmostBoxes;
    private Box[] _rightmostBoxes;
    private Box[] _allBoxes;

    internal void Initialize(GameBoard gameBoard)
    {
        _gameBoard = gameBoard;

        RemoveAllChildBoxes();
        GenerateNewShape();
        GetAndPlaceNewBoxes();
        RecalculateBoxes();
    }

    internal int[] GetUniqueBoxYPositions()
    {
        int[] yPositions = new int[_allBoxes.Length];

        for (int i = 0; i < _allBoxes.Length; i++)
        {
            yPositions[i] = (int) _allBoxes[i].transform.position.y;
        }

        return yPositions.Distinct().ToArray();
    }

    internal void RemoveBoxesWithYPosition(int yPosition)
    {
        Box[] matchingBoxes = _allBoxes.Where(x => (int) x.transform.position.y == yPosition).ToArray();

        if (matchingBoxes.Length == 0) { return; }

        foreach (Box box in matchingBoxes)
        {
            _gameBoard.AddBoxToPool(box);
        }

        RecalculateBoxes();
    }

    internal void SetHighlight(bool highlight)
    {
        foreach (var box in _allBoxes)
        {
            box.HighlightBox(highlight);
        }

        IsHighlighted = highlight;
    }

    internal void AttemptToMoveInDirection(Direction direction)
    {
        if (!CanMoveInDirection(direction)) { return; }

        switch (direction)
        {
            case Direction.Down:  transform.position += Vector3.down;  return;
            case Direction.Left:  transform.position += Vector3.left;  return;
            case Direction.Right: transform.position += Vector3.right; return;
        }
    }

    // This is how a rotated shape would be gotten I think (check GetTetrominoShape),
    // then you have to check if any of the positions are occupied, disregarding the boxes of this tetromino.
    // The positions would be checked the same way they're checked in GetAndPlaceNewBoxes();
    // private void Rotate(int degrees)
    // {
    //     var newShape = TetrominoShapeHelper.GetTetrominoShape(_tetrominoShape.Letter, _tetrominoShape.Rotation + degrees);
    // }

    private bool CanMoveInDirection(Direction direction)
    {
        Box[] boxesToCollisionCheck;

        switch (direction)
        {
            case Direction.Down:  boxesToCollisionCheck = _bottomBoxes;    break;
            case Direction.Left:  boxesToCollisionCheck = _leftmostBoxes;  break;
            case Direction.Right: boxesToCollisionCheck = _rightmostBoxes; break;

            default: throw new System.Exception($"Direction '{direction.ToString()}' not implemented");
        }

        foreach (var box in boxesToCollisionCheck)
        {
            if (!box.CanMoveInDirection(direction))
            {
                if (direction == Direction.Down) { _gameBoard.MakeTetrominoStatic(this); }
                return false;
            }
        }

        return true;
    }

    private void RecalculateBoxes()
    {
        _allBoxes = GetAllChildBoxes();

        _bottomBoxes    = GetBoxesToCollisionCheck(_allBoxes, Direction.Down);
        _leftmostBoxes  = GetBoxesToCollisionCheck(_allBoxes, Direction.Left);
        _rightmostBoxes = GetBoxesToCollisionCheck(_allBoxes, Direction.Right);
    }

    private void GenerateNewShape()
    {
        TetrominoShape newShape;
        do
        {
            newShape = TetrominoShapeHelper.GetRandomTetrominoShape();
        }
        while (_tetrominoShape == newShape);

        _tetrominoShape = newShape;
    }

    private Box[] GetAllChildBoxes()
    {
        Box[] childBoxes = new Box[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            childBoxes[i] = transform.GetChild(i).GetComponent<Box>();
        }

        return childBoxes;
    }

    /// <summary>
    /// Returns an array of the boxes from <paramref name="allBoxes"/>
    /// that does not have another box (from the array) directly next to it in the direction given.
    /// </summary>
    private Box[] GetBoxesToCollisionCheck(Box[] allBoxes, Direction direction)
    {
        switch (direction)
        {
            case Direction.Down:
                return allBoxes
                    .Where(a => !allBoxes
                           .Any(b => (int) b.transform.localPosition.x == (int) a.transform.localPosition.x
                                  && (int) b.transform.localPosition.y == (int) a.transform.localPosition.y - 1)).ToArray();

            case Direction.Left:
                return allBoxes
                    .Where(a => !allBoxes
                           .Any(b => (int) b.transform.localPosition.y == (int) a.transform.localPosition.y
                                  && (int) b.transform.localPosition.x == (int) a.transform.localPosition.x - 1)).ToArray();

            case Direction.Right:
                return allBoxes
                    .Where(a => !allBoxes
                           .Any(b => (int) b.transform.localPosition.y == (int) a.transform.localPosition.y
                                  && (int) b.transform.localPosition.x == (int) a.transform.localPosition.x + 1)).ToArray();

            default: throw new System.Exception($"Direction '{direction.ToString()}' not implemented");
        }
    }

    private void RemoveAllChildBoxes()
    {
        while (transform.childCount != 0)
        {
            // Since AddBoxToPool unchilds the box, this will iterate through all children.
            _gameBoard.AddBoxToPool(transform.GetChild(0).GetComponent<Box>());
        }
    }

    private void GetAndPlaceNewBoxes()
    {
        for (int y = 0; y < 4; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                var letter = _tetrominoShape.Shape[y, x];
                if (letter == ' ') { continue; }
                _gameBoard.GetDeactivatedBox().Activate(transform, new Vector2(x, -y), letter);
            }
        }
    }
}
