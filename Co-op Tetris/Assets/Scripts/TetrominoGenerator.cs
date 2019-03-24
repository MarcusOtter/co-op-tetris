using UnityEngine;

public class TetrominoGenerator : MonoBehaviour
{
    private const int TetrominoHeight = 4;

    [SerializeField] private Tetromino _tetrominoPrefab;

    private float _spawnDelayMax = 1;
    private float _spawnDelay;
    private int _tickCount = 4;

    private int _minSpawnPositionX;
    private int _maxSpawnPositionX;
    private int _spawnPositionY;

    private void Awake()
    {
        _spawnDelay = _spawnDelayMax;
    }

    private void Start()
    {
        SetSpawnPositions();
        GameBoard.OnTetrominoTick += CountTick;
    }

    private void CountTick(object sender, System.EventArgs e)
    {
        _tickCount++;
    }

    private void Update()
    {
        if (_spawnDelay > 0)
        {
            _spawnDelay -= Time.deltaTime;
            return;
        }

        if (_tickCount < TetrominoHeight) { return; }

        SpawnRandomTetromino();
        _tickCount = 0;
    }

    // Please note: following comments are scribbles at 3am

    // When the tetrominoes are moved in gameboard,
    // this should instead be a counter method that disables itself
    // from spawning more often than once every 4 calls
    // this has to be at least x4 the tick speed to prevent overlap.
    // alternatively store the x position and ensure the next one is at least 4 units away from this one in X axis
    // could even keep a list of all positions that were spawned in in the past 4 ticks and just try to calculate
    // a valid spawn position. 

    // another way is to just check if the spot is valid by calling CanMoveDown() on the spawned
    // tetromino, and if false just move it one step in either direction and try again
    // if it tries every single X and none is valid, game is over
    private void SpawnRandomTetromino()
    {
        int randomX = Random.Range(_minSpawnPositionX, _maxSpawnPositionX + 1);
        Instantiate(_tetrominoPrefab, new Vector3(randomX, _spawnPositionY, 0), Quaternion.identity);
    }

    private void SetSpawnPositions()
    {
        var board = FindObjectOfType<GameBoard>();
        _minSpawnPositionX = board.LeftBoundX + 1;
        _maxSpawnPositionX = board.RightBoundX - 4;
        _spawnPositionY = board.UpperBoundY + 4;
    }
}
