using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    internal static event EventHandler OnTetrominoTick;

    [Header("Prefab references")]
    [SerializeField] private Box _boxPrefab;

    [Header("Board size")]
    [SerializeField] private float _leftBoundX;
    [SerializeField] private float _rightBoundX;
    [SerializeField] private float _bottomBoundY;
    
    [Header("Fall speed")]
    [SerializeField] private float _defaultDelay = 1f;
    [SerializeField] private float _shortDelay = 0.025f;

    private Queue<Box> _pooledBoxes = new Queue<Box>();
    private List<Box> _enabledBoxes = new List<Box>();

    private bool _gameIsActive = true;

    private Coroutine _activeTick;

    private void Awake()
    {
        IncreasePoolCapacity(40);
        _activeTick = StartCoroutine(DefaultTickDelay());
    }

    private bool _isHoldingSpace;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
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
            OnTetrominoTick?.Invoke(this, EventArgs.Empty);
            yield return new WaitForSeconds(_defaultDelay);
        }
    }

    private IEnumerator ShortTickDelay()
    {
        while (_gameIsActive)
        {
            OnTetrominoTick?.Invoke(this, EventArgs.Empty);
            yield return new WaitForSeconds(_shortDelay);
        }
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
        if (tilePosition.x <= _leftBoundX) { return true; }
        if (tilePosition.x >= _rightBoundX) { return true; }
        if (tilePosition.y <= _bottomBoundY) { return true; }

        // Checks if any boxes are already in this position
        foreach (var box in _enabledBoxes)
        {
            // If X doesn't match, Y doesn't have to be checked (and vice versa)
            if ((int) box.transform.position.x != tilePosition.x) { continue; }

            // Both X and Y matches, which means this tile is occupied
            if ((int) box.transform.position.y == tilePosition.y) { return true; }
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
