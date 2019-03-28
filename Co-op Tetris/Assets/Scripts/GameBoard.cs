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

    private List<Tetromino> _activeTetrominoes = new List<Tetromino>();
    private List<Tetromino> _deactivatedTetrominoes = new List<Tetromino>();

    private bool _gameIsActive = true;
    private bool _isHoldingSpace;

    private Coroutine _activeTick;

    private void Awake()
    {
        IncreasePoolCapacity(80);
        _activeTick = StartCoroutine(DefaultTickDelay());
    }

    private void Update()
    {
        // This should be moved to an input class later on.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Slow tick should always run.
            // Fast tick should only run on the highlighted box
            StopCoroutine(_activeTick);
            _activeTick = StartCoroutine(ShortTickDelay());
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            StopCoroutine(_activeTick);
            _activeTick = StartCoroutine(DefaultTickDelay());
        }
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

    private void MoveActiveTetrominoesDown()
    {
        foreach (var tetromino in _activeTetrominoes.OrderBy(x => x.transform.position.y))
        {
            tetromino.AttemptDescent();
        }
    }

    internal void ActivateTetromino(Tetromino tetromino)
    {
        if (_deactivatedTetrominoes.Contains(tetromino))
        {
            _deactivatedTetrominoes.Remove(tetromino);
        }

        _activeTetrominoes.Add(tetromino);
    }

    // Add amount of boxes to like an active boxes thing or something?
    // Tetrominoes should be deactivated once they reach the ground, but
    // then a method would check if the tetromino has any boxes above the
    // line that was removed (tetromino.HasBoxWithYPosition()). If so, make
    // the tetromino active again.
    internal void DeactivateTetromino(Tetromino tetromino)
    {
        if (_activeTetrominoes.Contains(tetromino))
        {
            _activeTetrominoes.Remove(tetromino);
        }

        _deactivatedTetrominoes.Add(tetromino);
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
        if (tilePosition.x <= LeftBoundX) { return true; }
        if (tilePosition.x >= RightBoundX) { return true; }
        if (tilePosition.y <= BottomBoundY) { return true; }

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

    private void IncreasePoolCapacity(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            var newObject = Instantiate(_boxPrefab);
            AddBoxToPool(newObject);
        }
    }
}
