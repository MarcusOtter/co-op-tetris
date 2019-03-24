using System;
using System.Linq;
using UnityEngine;

public class Tetromino : MonoBehaviour
{
    public bool Highlighted; // public for testing purposes

    private TetrominoShape _tetrominoShape;
    private GameBoard _gameBoard;

    private Box[] _boxesToCollisionCheck;
    private Box[] _boxes;

    private void Start()
    {
        _gameBoard = FindObjectOfType<GameBoard>();
        GameBoard.OnTetrominoTick += AttemptDescent;

        RemoveAllChildren();
        GenerateNewShape();
        DrawTetromino();
    }

    internal void RemoveBoxesWithYPosition(int yPosition)
    {
        Box[] matchingBoxes = _boxes.Where(x => (int) x.transform.position.y == yPosition).ToArray();

        if (matchingBoxes.Length == 0) { return; }

        foreach (Box box in matchingBoxes)
        {
            _gameBoard.AddBoxToPool(box);
        }

        _boxes = GetAllChildBoxes();
        _boxesToCollisionCheck = GetBoxesToCollisionCheck(_boxes);
    }

    private void AttemptDescent(object sender, EventArgs e)
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
                // Tell the gameBoard that this tetromino is now static
                // Check if the game is lost by iterating through the boxes and check Y position
                // (that can be done by the gameBoard)
                return false;
            }
        }

        return true;
    }

    /*
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Recalculates the shape of the tetromino
            RemoveAllChildren();
            GenerateNewShape();
            DrawTetromino();
        }
    }
    */

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

    // Should probably be renamed to convey that
    // it's getting boxes and activating them if the 
    // character in _tetrominoShape.Shape is not the space character.
    // Also currently recalculates _boxes and _boxesToCollisionCheck.
    private void DrawTetromino()
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

        // Move this outside of here too?
        _boxes = GetAllChildBoxes();
        _boxesToCollisionCheck = GetBoxesToCollisionCheck(_boxes);

        // Should be moved to separate method
        if (Highlighted)
        {
            foreach(var box in _boxes)
            {
                box.HighlightBox(true);
            }
        }
    }
}
