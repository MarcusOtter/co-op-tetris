using System.Linq;
using UnityEngine;

public class Tetromino : MonoBehaviour
{
    internal bool IsHighlighted { get; private set; }
    internal int BoxAmount => _boxes.Length;

    private TetrominoShape _tetrominoShape;
    private GameBoard _gameBoard;

    private Box[] _boxesToCollisionCheck;
    private Box[] _boxes;

    internal void Initialize(GameBoard gameBoard)
    {
        _gameBoard = gameBoard;

        RemoveAllChildren();
        GenerateNewShape();
        GetAndPlaceNewBoxes();
        RecalculateBoxes();
    }

    internal int[] GetUniqueBoxYPositions()
    {
        int[] yPositions = new int[_boxes.Length];

        for (int i = 0; i < _boxes.Length; i++)
        {
            yPositions[i] = (int) _boxes[i].transform.position.y;
        }

        return yPositions.Distinct().OrderBy(x => x).ToArray();
    }

    internal void RemoveBoxesWithYPosition(int yPosition)
    {
        Box[] matchingBoxes = _boxes.Where(x => (int) x.transform.position.y == yPosition).ToArray();

        if (matchingBoxes.Length == 0) { return; }

        foreach (Box box in matchingBoxes)
        {
            _gameBoard.AddBoxToPool(box);
        }

        RecalculateBoxes();
    }

    internal void SetHighlight(bool highlight)
    {
        foreach (var box in _boxes)
        {
            box.HighlightBox(highlight);
        }

        IsHighlighted = highlight;
    }

    internal void AttemptDescent()
    {
        if (!CanMoveDown()) { return; }
        transform.position = new Vector3(transform.position.x, transform.position.y - 1, 0);
    }

    private bool CanMoveDown()
    {
        foreach (var box in _boxesToCollisionCheck)
        {
            if (!box.CanMoveDown)
            {
                // Important for performance, but currently removes
                // a gameplay mechanic (read more over GameBoard.MakeTetrominoStatic)
                _gameBoard.MakeTetrominoStatic(this);
                return false;
            }
        }

        return true;
    }

    private void RecalculateBoxes()
    {
        _boxes = GetAllChildBoxes();
        _boxesToCollisionCheck = GetBoxesToCollisionCheck(_boxes);
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
    /// that does not have another box (from the <paramref name="allBoxes"/> array) directly under it.
    /// </summary>
    private Box[] GetBoxesToCollisionCheck(Box[] allBoxes)
    {
        // Yes, a bit messy. But this is my sweet little baby, and she's fully functional.
        Box[] boxesToCollisionCheck = allBoxes
            .Where(a => !allBoxes
                   .Any(b => (int) b.transform.localPosition.x == (int) a.transform.localPosition.x
                          && (int) b.transform.localPosition.y == (int) a.transform.localPosition.y - 1))
            .ToArray();

        return boxesToCollisionCheck;
    }

    private void RemoveAllChildren()
    {
        while (transform.childCount != 0)
        {
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

                var box = _gameBoard.GetDeactivatedBox();
                box.Activate(transform, new Vector2(x, -y), letter);
            }
        }
    }
}
