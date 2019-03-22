using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    internal static event EventHandler OnTetrominoTick;

    [Header("Prefab references")]
    [SerializeField] private GameObject _boxPrefab;

    [Header("Board size")]
    [SerializeField] private float _leftBoundX;
    [SerializeField] private float _rightBoundX;
    [SerializeField] private float _bottomBoundY;

    private Queue<GameObject> _pooledBoxes = new Queue<GameObject>();
    private List<GameObject> _enabledBoxes = new List<GameObject>();

    private bool _gameIsActive = true;

    private void Awake()
    {
        IncreasePoolCapacity(40);
        StartCoroutine(TetrominoTick());
    }

    private IEnumerator TetrominoTick()
    {
        while (_gameIsActive)
        {
            OnTetrominoTick?.Invoke(this, EventArgs.Empty);
            yield return new WaitForSeconds(1f);
        }
    }

    internal GameObject GetBox()
    {
        if (_pooledBoxes.Count == 0)
        {
            IncreasePoolCapacity(1);
        }

        var boxToUnpool = _pooledBoxes.Dequeue();
        _enabledBoxes.Add(boxToUnpool);
        boxToUnpool.SetActive(true);

        return boxToUnpool;
    }

    internal void AddBoxToPool(GameObject boxToPool)
    {
        boxToPool.SetActive(false);
        boxToPool.transform.parent = transform;

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
