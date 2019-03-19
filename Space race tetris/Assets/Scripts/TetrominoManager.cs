using System.Collections.Generic;
using UnityEngine;

public class TetrominoManager : MonoBehaviour 
{
    private List<Transform> _activeTetrominoBoxes = new List<Transform>();
    private GridBox[,] _gameBoard = new GridBox[20, 20];
    private int _boardSize = 20;

   	private void Awake() 
    {
        _gameBoard = new GridBox[20, 20];
        InitializeGameBoard();
	}

    // active tetrominoes
    // try move down (internal logic returns bool)
    // if false remove from active
    // maybe report back the position too with a <> tuple

    private void InitializeGameBoard()
    {
        for (int x = 0; x < _boardSize; x++)
        {
            for (int y = 0; y < _boardSize; y++)
            {
                _gameBoard[x, y] = new GridBox(new Vector2Int(x, y));
            }
        }
    }

    private void Update()
    {
        // Should not be in update!!

        foreach (var tetrominoBox in _activeTetrominoBoxes)
        {
            for (int x = 0; x < _boardSize; x++)
            {
                for (int y = 0; y < _boardSize; y++)
                {
                    _gameBoard[x, y].IsOccupied = ((Vector2) tetrominoBox.position) == _gameBoard[x, y].Position;
                }
            }
        }
    }

    internal bool BoxIsOccupied(Vector2Int position)
    {
        if (position.y <= 0) { return true; }

        return _gameBoard[position.x, position.y].IsOccupied;
    }
}
