using UnityEngine;

public class TetrominoGenerator : MonoBehaviour
{
    private const int TetrominoHeight = 4;

    [SerializeField] private Tetromino _tetrominoPrefab;
    [SerializeField] private float _spawnDelayMax = 2;

    private GameBoard _gameBoard;

    private float _spawnDelay;
    private int _tickCount = 4;

    private int _minSpawnPositionX;
    private int _maxSpawnPositionX;
    private int _spawnPositionY;

    private void OnEnable()
    {
        GameBoard.OnTetrominoTick += CountTick;
    }

    private void Start()
    {
        _gameBoard = FindObjectOfType<GameBoard>();
        SetSpawnPositions();
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
        _spawnDelay = _spawnDelayMax;
    }

    private void SpawnRandomTetromino()
    {
        int randomX = Random.Range(_minSpawnPositionX, _maxSpawnPositionX + 1);
        var newTetromino = Instantiate(_tetrominoPrefab, new Vector3(randomX, _spawnPositionY, 0), Quaternion.identity, transform);
        newTetromino.Initialize(_gameBoard);
        _gameBoard.MakeTetrominoFalling(newTetromino);
    }

    private void SetSpawnPositions()
    {
        // Ensures that no tetromino will spawn outside the bounds of the walls
        _minSpawnPositionX = _gameBoard.LeftBoundX + 1;
        _maxSpawnPositionX = _gameBoard.RightBoundX - 4;
        _spawnPositionY = _gameBoard.UpperBoundY + 4;
    }

    private void OnDisable()
    {
        GameBoard.OnTetrominoTick -= CountTick;
    }
}
