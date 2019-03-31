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
        InputManager.OnDownKeyDown += ActivateShortTick;
        InputManager.OnDownKeyUp += ActivateSlowTick;
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

    internal void MakeTetrominoFalling(Tetromino tetromino)
    {
        if (_staticTetrominoes.Contains(tetromino))
        {
            _staticTetrominoes.Remove(tetromino);
        }

        _fallingTetrominoes.Add(tetromino);
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

        _staticTetrominoes.Add(tetromino);
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

        var boxesToShift = _enabledBoxes
            .Where(x => x.transform.position.y > yPosition)
            .OrderBy(x => x.transform.position.y)
            .ToArray();

        foreach (var box in boxesToShift)
        {
            box.transform.position 
                = new Vector3(box.transform.position.x, box.transform.position.y - 1, 0);
        }

        ClearEmptyTetrominoes();
    }

    // Used after removing a row of boxes to clear up empty tetrominoes
    private void ClearEmptyTetrominoes()
    {
        List<Tetromino> emptyTetrominoes = new List<Tetromino>();

        foreach(var tetromino in _fallingTetrominoes)
        {
            if (tetromino.BoxAmount == 0) { emptyTetrominoes.Add(tetromino); }
        }

        foreach(var tetromino in _staticTetrominoes)
        {
            if (tetromino.BoxAmount == 0) { emptyTetrominoes.Add(tetromino); }
        }

        foreach(var emptyTetromino in emptyTetrominoes)
        {
            DestroyTetromino(emptyTetromino);
        }
    }

    internal void DestroyTetromino(Tetromino tetromino)
    {
        if (_fallingTetrominoes.Contains(tetromino))
        {
            _fallingTetrominoes.Remove(tetromino);
        }

        if (_staticTetrominoes.Contains(tetromino))
        {
            _fallingTetrominoes.Remove(tetromino);
        }

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
    private void ActivateShortTick(object sender, EventArgs e)
    {
        StopCoroutine(_activeTick);
        _activeTick = StartCoroutine(ShortTickDelay());
    }

    private void ActivateSlowTick(object sender, EventArgs e)
    {
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
            var newObject = Instantiate(_boxPrefab);
            AddBoxToPool(newObject);
        }
    }

    private void OnDisable()
    {
        InputManager.OnDownKeyDown -= ActivateShortTick;
        InputManager.OnDownKeyUp -= ActivateSlowTick;
    }
}
