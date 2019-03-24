using System.Collections;
using UnityEngine;

public class TetrominoGenerator : MonoBehaviour
{
    [SerializeField] private Tetromino _tetrominoPrefab;

    private int _minSpawnPositionX;
    private int _maxSpawnPositionX;
    private int _spawnPositionY;

    private void Start()
    {
        SetSpawnPositions();
        StartCoroutine(SpawnTetrominoes());
    }

    // When the tetrominoes are moved in gameboard,
    // this should instead be a counter method that disables itself
    // from spawning more often than once every 4 calls
    private IEnumerator SpawnTetrominoes()
    {
        while (true)
        {
            int randomX = Random.Range(_minSpawnPositionX, _maxSpawnPositionX + 1);
            Instantiate(_tetrominoPrefab, new Vector3(randomX, _spawnPositionY, 0), Quaternion.identity);
            yield return new WaitForSeconds(4f); // this has to be at least x4 the tick speed to prevent overlap.
            // alternatively store the x position and ensure the next one is at least 4 units away from this one in X axis
            // could even keep a list of all positions that were spawned in in the past 4 ticks and just try to calculate
            // a valid spawn position. 

            // another way is to just check if the spot is valid by calling CanMoveDown() on the spawned
            // tetromino, and if false just move it one step in either direction and try again
            // if it tries every single X and none is valid, game is over
        }
    }

    private void SetSpawnPositions()
    {
        var board = FindObjectOfType<GameBoard>();
        _minSpawnPositionX = board.LeftBoundX + 1;
        _maxSpawnPositionX = board.RightBoundX - 4;
        _spawnPositionY = board.UpperBoundY + 4;
    }
}
