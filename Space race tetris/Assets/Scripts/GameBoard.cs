using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    internal static event EventHandler OnTetrominoTick;

    [SerializeField] private GameObject _boxPrefab;

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
        foreach (var box in _enabledBoxes)
        {
            if (box.transform.position.x != tilePosition.x) { continue; }
            if (box.transform.position.y != tilePosition.y) { continue; }

            return true;
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
