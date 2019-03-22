using System;
using System.Linq;
using UnityEngine;

public class Tetromino : MonoBehaviour
{
    public bool IsFalling;

    private TetrominoShape _tetrominoShape;
    private GameBoard _gameBoard;

    private void Start()
    {
        _gameBoard = FindObjectOfType<GameBoard>();
        GameBoard.OnTetrominoTick += AttemptDescent;

        RemoveAllChildren();
        GenerateNewShape();
        DrawTetromino();
    }

    // TODO: Fix ultra hacky implementation
    private void AttemptDescent(object sender, EventArgs e)
    {
        if (!IsFalling) return;

        var legalMove = true;

        Transform[] children = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            children[i] = transform.GetChild(i);
        }

        children = children.OrderBy(x => x.localPosition.y).ToArray();
        Transform[] lowestChildren = children.Where(x => x.localPosition.y == children[0].localPosition.y).ToArray();

        // THIS DOES NOT WORK FOR SOME PIECES:
        // THE TETROMINO NEEDS TO CHECK EVERY BOX THAT DOESN'T HAVE A BOX OF THIS TETROMINO UNDER IT

        foreach (var child in lowestChildren)
        {
            if (_gameBoard.TileIsOccupied(new Vector2Int((int) child.position.x, (int) child.position.y - 1)))
            {
                legalMove = false;
            }
        }

        if (legalMove)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - 1, 0);
        }
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

    /*
    private void Start()
    {
        _tetrominoManager = FindObjectOfType<TetrominoManager>();
    }

    private void MoveDown()
    {
        foreach (var box in _tetrominoBoxes)
        {
            if (_tetrominoManager.BoxIsOccupied(new Vector2Int((int) box.position.x, (int) box.position.y - _stepDistance))) { return; }
        }

        transform.position = new Vector3(transform.position.x, transform.position.y - _stepDistance, 0);
    }


    private IEnumerator TempSteps()
    {
        while (_isFalling)
        {
            yield return new WaitForSeconds(_stepDelay);

            MoveDown();
        }
    }
    */
}
