using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameBoard : MonoBehaviour
{
    // For tetromino generator and (eventually) points.
    internal static event EventHandler OnTetrominoTick; 

    [Header("Prefab references")]
    [SerializeField] private Box _boxPrefab;

    [Header("Board size")]
    [SerializeField] internal int LeftBoundX = 1;
    [SerializeField] internal int RightBoundX = 41;
    [SerializeField] internal int UpperBoundY = 23;
    [SerializeField] internal int BottomBoundY = 1;
    
    [Header("Fall speed")]
    [SerializeField] private float _defaultDelay = 1f;
    [SerializeField] private float _shortDelay = 0.025f;

    private Queue<Box> _pooledBoxes = new Queue<Box>();
    private List<Box> _enabledBoxes = new List<Box>();

    private List<Tetromino> _fallingTetrominoes = new List<Tetromino>();
    private List<Tetromino> _staticTetrominoes = new List<Tetromino>();

    private bool _gameIsActive = true;
    private bool _isHoldingSpace;

    private Coroutine _activeTick;

    private void Awake()
    {
        IncreasePoolCapacity(256);
        _activeTick = StartCoroutine(DefaultTickDelay());
    }

    private void OnEnable()
    {
        InputManager.OnDownDirectionPressed += ActivateShortTick;
        InputManager.OnDownDirectionReleased += ActivateSlowTick;
    }

    internal Box GetDeactivatedBox()
    {
        if (_pooledBoxes.Count == 0)
        {
            IncreasePoolCapacity(1);
        }

        var boxToUnpool = _pooledBoxes.Dequeue();
        _enabledBoxes.Add(boxToUnpool);

        return boxToUnpool;
    }

    internal void AddBoxToPool(Box boxToPool)
    {
        boxToPool.Deactivate();

        if (_enabledBoxes.Contains(boxToPool))
        {
            _enabledBoxes.Remove(boxToPool);
        }

        _pooledBoxes.Enqueue(boxToPool);
    }

    internal bool TileIsOccupied(Vector2Int tilePosition)
    {
        // Ensure tile is within the board bounds
        if (tilePosition.y <= BottomBoundY) { return true; }
        if (tilePosition.x <= LeftBoundX)   { return true; }
        if (tilePosition.x >= RightBoundX)  { return true; }

        // Checks if any boxes are already in this position
        foreach (var box in _enabledBoxes)
        {
            // If X doesn't match, Y doesn't have to be checked (and vice versa)
            if (box.transform.position.x != tilePosition.x) { continue; }

            // Both X and Y matches, which means this tile is occupied
            if (box.transform.position.y == tilePosition.y) { return true; }
        }

        return false;
    }

    internal void AddNewTetromino(Tetromino tetromino)
    {
        if (!_fallingTetrominoes.Any())
        {
            tetromino.SetHighlight(true);
        }

        MakeTetrominoFalling(tetromino);
    }

    // Tetrominoes should be deactivated once they reach the ground, but
    // then a method would force the tetromino to verify that it should still
    // be deactivate again once a row is shifted or cleared. 
    // Should be done from the ground up as usual.
    internal void MakeTetrominoStatic(Tetromino tetromino)
    {
        if (_fallingTetrominoes.Contains(tetromino))
        {
            _fallingTetrominoes.Remove(tetromino);
        }

        if (tetromino.IsHighlighted)
        {
            tetromino.SetHighlight(false);
            HighlightLowestFallingTetromino();
        }

        _staticTetrominoes.Add(tetromino);

        // Check the modified rows and remove them if they are filled up
        foreach(int yPos in tetromino.GetUniqueBoxYPositions())
        {
            if (RowIsFull(yPos)) { RemoveRow(yPos); }
        }
    }

    private void MakeTetrominoFalling(Tetromino tetromino)
    {
        if (_staticTetrominoes.Contains(tetromino))
        {
            _staticTetrominoes.Remove(tetromino);
        }

        if (_fallingTetrominoes.Contains(tetromino))
        {
            return;
        }

        _fallingTetrominoes.Add(tetromino);
    }

    private void HighlightLowestFallingTetromino()
    {
        if (!_fallingTetrominoes.Any()) { return; }

        _fallingTetrominoes
            .OrderBy(x => x.transform.position.y)
            .FirstOrDefault()
            .SetHighlight(true);
    }

    private bool RowIsFull(int yPosition)
    {
        int fullRowBoxAmount = RightBoundX - LeftBoundX - 1;
        return _enabledBoxes
            .Where(x => x.transform.position.y == yPosition)
            .Count() == fullRowBoxAmount;
    }

    private void RemoveRow(int yPosition)
    {
        foreach (var tetromino in _fallingTetrominoes)
        {
            tetromino.RemoveBoxesWithYPosition(yPosition);
        }

        foreach (var tetromino in _staticTetrominoes)
        {
            tetromino.RemoveBoxesWithYPosition(yPosition);
        }

        // TODO: Extract method
        var boxesToSetFalling = _enabledBoxes
            .Where(x => x.transform.position.y >= yPosition)
            .OrderBy(x => x.transform.position.y)
            .ToArray();

        foreach (var box in boxesToSetFalling)
        {
            MakeTetrominoFalling(box.ParentTetromino);
        }

        // TODO: Should be extracted to separate method to shift boxes down
        //var boxesToShift = _enabledBoxes
        //    .Where(x => x.transform.position.y > yPosition)
        //    .OrderBy(x => x.transform.position.y)
        //    .ToArray();

        //foreach (var box in boxesToShift)
        //{
        //    box.transform.position 
        //        = new Vector3(box.transform.position.x, box.transform.position.y - 1, 0);
        //}

        ClearEmptyTetrominoes();
    }

    private void ClearEmptyTetrominoes()
    {
        List<Tetromino> emptyTetrominoes = new List<Tetromino>();

        emptyTetrominoes.AddRange(_fallingTetrominoes.Where(x => x.BoxAmount == 0));
        emptyTetrominoes.AddRange(_staticTetrominoes .Where(x => x.BoxAmount == 0));

        foreach(var emptyTetromino in emptyTetrominoes)
        {
            DestroyTetromino(emptyTetromino);
        }
    }

    private void DestroyTetromino(Tetromino tetromino)
    {
        if (_fallingTetrominoes.Contains(tetromino))
        {
            _fallingTetrominoes.Remove(tetromino);
        }

        if (_staticTetrominoes.Contains(tetromino))
        {
            _fallingTetrominoes.Remove(tetromino);
        }

        if (tetromino == null) { return; }
        Destroy(tetromino.gameObject);
    }

    private IEnumerator DefaultTickDelay()
    {
        while (_gameIsActive)
        {
            MoveActiveTetrominoesDown();
            OnTetrominoTick?.Invoke(this, EventArgs.Empty);
            yield return new WaitForSeconds(_defaultDelay);
        }
    }

    private IEnumerator ShortTickDelay()
    {
        while (_gameIsActive)
        {
            MoveActiveTetrominoesDown();
            OnTetrominoTick?.Invoke(this, EventArgs.Empty);
            yield return new WaitForSeconds(_shortDelay);
        }
    }

    // Slow tick should always run.
    // Fast tick should only run on the highlighted tetromino (eventually)
    private void ActivateShortTick(object sender, int playerNumber)
    {
        if (playerNumber != 1) { return; } // temp

        StopCoroutine(_activeTick);
        _activeTick = StartCoroutine(ShortTickDelay());
    }

    private void ActivateSlowTick(object sender, int playerNumber)
    {
        if (playerNumber != 1) { return; } // temp

        StopCoroutine(_activeTick);
        _activeTick = StartCoroutine(DefaultTickDelay());
    }

    private void MoveActiveTetrominoesDown()
    {
        foreach (var tetromino in _fallingTetrominoes.OrderBy(x => x.transform.position.y))
        {
            tetromino.AttemptDescent();
        }
    }

    private void IncreasePoolCapacity(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            var newBox= Instantiate(_boxPrefab);
            newBox.Initialize(this);
            AddBoxToPool(newBox);
        }
    }

    private void OnDisable()
    {
        InputManager.OnDownDirectionPressed -= ActivateShortTick;
        InputManager.OnDownDirectionReleased -= ActivateSlowTick;
    }
}
