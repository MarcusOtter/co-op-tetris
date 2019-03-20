using System.Collections.Generic;
using UnityEngine;

public class BoxPool : MonoBehaviour
{
    [SerializeField] private GameObject _boxPrefab;

    private Queue<GameObject> _boxesInPool = new Queue<GameObject>();

    private void Awake()
    {
        IncreasePoolCapacity(40);
    }

    internal GameObject GetBox()
    {
        if (_boxesInPool.Count == 0)
        {
            IncreasePoolCapacity(1);
        }

        return _boxesInPool.Dequeue();
    }

    internal void AddBoxToPool(GameObject boxToPool)
    {
        boxToPool.gameObject.SetActive(false);
        boxToPool.transform.parent = transform;
        _boxesInPool.Enqueue(boxToPool);
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
