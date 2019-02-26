using System.Collections;
using UnityEngine;

public class Tetromino : MonoBehaviour
{
    [SerializeField] private float _stepDistance = 1f;
    [SerializeField] private float _stepDelay = 2f;

    private bool _isFalling = true;

    private void Awake()
    {
        StartCoroutine(TempSteps());
    }

    private IEnumerator TempSteps()
    {
        while (_isFalling)
        {
            yield return new WaitForSeconds(_stepDelay);
            transform.position = new Vector3(transform.position.x, transform.position.y - _stepDistance, 0);           
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        _isFalling = false;
    }
}
