using System;
using System.Linq;
using UnityEngine;

public class Tetromino : MonoBehaviour
{
    private TetrominoShape _tetrominoShape;
    private GameBoard _gameBoard;

    private Transform[] _boxesToCollisionCheck;
    private Transform[] _boxes;

    private void Start()
    {
        _gameBoard = FindObjectOfType<GameBoard>();
        GameBoard.OnTetrominoTick += AttemptDescent;

        RemoveAllChildren();
        GenerateNewShape();
        DrawTetromino();
    }

    private void AttemptDescent(object sender, EventArgs e)
    {
        foreach (var box in _boxesToCollisionCheck)
        {
            if (_gameBoard.TileIsOccupied(new Vector2Int((int) box.position.x, (int) box.position.y - 1)))
            {
                return;
            }
        }

        transform.position = new Vector3(transform.position.x, transform.position.y - 1, 0);
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
            newShape = new TetrominoShape();
        }
        while (_tetrominoShape == newShape);

        _tetrominoShape = newShape;
    }

    private Transform[] GetAllChildBoxes()
    {
        Transform[] childBoxes = new Transform[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            childBoxes[i] = transform.GetChild(i);
        }

        return childBoxes;
    }

    /// <summary>
    /// Returns an array of the boxes from <paramref name="allBoxes"/>
    /// that does not have another box (from the <paramref name="allBoxes"/> array) under it.
    /// </summary>
    private Transform[] GetBoxesToCollisionCheck(Transform[] allBoxes)
    {
        Transform[] boxesToCollisionCheck = allBoxes
            .Where(a => !allBoxes
                   .Any(b => (int) b.localPosition.x == (int) a.localPosition.x
                          && (int) b.localPosition.y == (int) a.localPosition.y - 1))
            .ToArray();

        return boxesToCollisionCheck;
    }

    private void RemoveAllChildren()
    {
        while (transform.childCount != 0)
        {
            _gameBoard.AddBoxToPool(transform.GetChild(0).gameObject);
        }
    }

    private void DrawTetromino()
    {
        for (int y = 0; y < 4; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                var letter = _tetrominoShape.Shape[y, x];
                if (letter == ' ') { continue; }

                var box = _gameBoard.GetBox();
                box.transform.SetParent(transform);
                box.transform.localPosition = new Vector2(x, -y);
                box.GetComponent<SpriteRenderer>().color = GetColorFromLetter(letter);
                box.gameObject.SetActive(true);
            }
        }

        _boxes = GetAllChildBoxes();
        _boxesToCollisionCheck = GetBoxesToCollisionCheck(_boxes);
    }

    private Color GetColorFromLetter(char letter)
    {
        switch (letter)
        {
            case 'C': return Color.cyan;
            case 'B': return Color.blue;
            case 'O': return new Color32(232, 144, 0, 255); // Orange
            case 'Y': return Color.yellow;
            case 'G': return Color.green;
            case 'R': return Color.red;
            case 'P': return new Color32(140, 50, 255, 255); // Purple

            default: throw new Exception($"There is no color that corresponds with the letter '{letter}'");
        }
    }
}
