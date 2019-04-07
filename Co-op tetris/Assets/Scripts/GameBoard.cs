using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameBoard : MonoBehaviour
{
    [Header("Prefab references")]
    [SerializeField] private Box _boxPrefab;

    [Header("Board size")]
    [SerializeField] internal int LeftBoundX = 1;
    [SerializeField] internal int RightBoundX = 41;
    [SerializeField] internal int UpperBoundY = 23;
    [SerializeField] internal int BottomBoundY = 1;
    
    [Header("Fall speed")]
    [SerializeField] private float _defaultTickDelay = 1f;
    [SerializeField] private float _shortTickDelay = 0.05f;

    private Queue<Box> _pooledBoxes = new Queue<Box>();
    private List<Box> _enabledBoxes = new List<Box>();

    private List<Tetromino> _fallingTetrominoes = new List<Tetromino>();
    private List<Tetromino> _staticTetrominoes = new List<Tetromino>();

    private Tetromino _highlightedTetromino;

    private bool _usingShortTick;
    private float _timeUntilDefaultTick;
    private float _timeUntilShortTick;

    private TetrominoGenerator _tetrominoGenerator;

    private void Awake()
    {
        IncreasePoolCapacity(256);
    }

    private void OnEnable()
    {
        InputManager.OnPlayerInput += HandlePlayerInput;
    }

    private void Start()
    {
        _tetrominoGenerator = FindObjectOfType<TetrominoGenerator>();
    }

    private void Update()
    {
        UpdateTetrominoTimers();
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

    internal void AddNewTetromino(Tetromino tetromino)
    {
        if (!_fallingTetrominoes.Any())
        {
            HighlightTetromino(tetromino, true);
        }

        MakeTetrominoFalling(tetromino);
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

        if (tetromino.IsHighlighted)
        {
            HighlightTetromino(tetromino, false);
            HighlightLowestFallingTetromino();
        }

        InputManager.SwapTetrominoPlayer();

        _staticTetrominoes.Add(tetromino);

        var fullRows = tetromino.GetUniqueBoxYPositions().Where(x => RowIsFull(x)).ToArray();
        RemoveRows(fullRows);
    }

    private void HandlePlayerInput(object sender, PlayerInputEventArgs eventArgs)
    {
        if (eventArgs.PlayerNumber != InputManager.TetrominoPlayerNumber) { return; }

        if (eventArgs.InputState == InputState.Pressed)
        {
            HandleInputPressed(eventArgs.InputAction);
        }
        else if (eventArgs.InputState == InputState.Released)
        {
            HandleInputReleased(eventArgs.InputAction);
        }
    }

    private void HandleInputPressed(PlayerInputAction action)
    {
        switch (action)
        {
            case PlayerInputAction.Right:
                _highlightedTetromino?.AttemptToMoveInDirection(Direction.Right);
                break;

            case PlayerInputAction.Left:
                _highlightedTetromino?.AttemptToMoveInDirection(Direction.Left);
                break;

            case PlayerInputAction.Down:
                _usingShortTick = true;
                break;
        }
    }

    private void HandleInputReleased(PlayerInputAction action)
    {
        switch (action)
        {
            case PlayerInputAction.Down:
                _usingShortTick = false;
                break;
        }
    }

    private void UpdateTetrominoTimers()
    {
        UpdateDefaultTick();

        if (_usingShortTick)
        {
            UpdateShortTick();
        }
    }

    private void UpdateShortTick()
    {
        if (_timeUntilShortTick > 0)
        {
            _timeUntilShortTick -= Time.deltaTime;
        }
        else
        {
            ShortTick();
            _timeUntilShortTick = _shortTickDelay;
        }
    }

    private void UpdateDefaultTick()
    {
        if (_timeUntilDefaultTick > 0)
        {
            _timeUntilDefaultTick -= Time.deltaTime;
        }
        else
        {
            DefaultTick();
            _timeUntilDefaultTick = _defaultTickDelay;
        }
    }

    private void DefaultTick()
    {
        MoveActiveTetrominoesDown();
        _tetrominoGenerator.CountTick();
    }

    private void ShortTick()
    {
        MoveHighlightedTetrominoDown();
    }

    private void MakeTetrominoFalling(Tetromino tetromino)
    {
        if (_staticTetrominoes.Contains(tetromino))
        {
            _staticTetrominoes.Remove(tetromino);
        }

        if (_fallingTetrominoes.Contains(tetromino))
        {
            return;
        }

        _fallingTetrominoes.Add(tetromino);
    }

    private void HighlightTetromino(Tetromino tetromino, bool highlight)
    {
        if (tetromino.IsHighlighted == highlight) { return;  }

        if (highlight)
        {
            _highlightedTetromino = tetromino;
        }
        else
        {
            _highlightedTetromino = null;
        }

        _usingShortTick = false;
        tetromino.SetHighlight(highlight);
    }

    private void HighlightLowestFallingTetromino()
    {
        if (!_fallingTetrominoes.Any()) { return; }
        HighlightTetromino(_fallingTetrominoes.OrderBy(x => x.transform.position.y).FirstOrDefault(), true);
    }

    private bool RowIsFull(int yPosition)
    {
        int fullRowBoxAmount = RightBoundX - LeftBoundX - 1;
        return _enabledBoxes
            .Where(x => x.transform.position.y == yPosition)
            .Count() == fullRowBoxAmount;
    }

    private void RemoveRows(int[] yPositions)
    {
        if (yPositions.Length == 0) { return; }

        yPositions = yPositions.OrderBy(x => x).ToArray();

        foreach (int yPosition in yPositions)
        {
            // Is this loop really needed?
            //foreach (var tetromino in _fallingTetrominoes)
            //{
            //    tetromino.RemoveBoxesWithYPosition(yPosition);
            //}

            foreach (var tetromino in _staticTetrominoes)
            {
                tetromino.RemoveBoxesWithYPosition(yPosition);
            }
        }

        var boxesToShift = _enabledBoxes
            .Where(x => x.transform.position.y > yPositions.Last())
            .OrderBy(x => x.transform.position.y)
            .ToArray();

        foreach (var box in boxesToShift)
        {
            box.transform.position += Vector3.down * yPositions.Length;
        }

        ClearEmptyTetrominoes();
    }

    private void ClearEmptyTetrominoes()
    {
        List<Tetromino> emptyTetrominoes = new List<Tetromino>();

        emptyTetrominoes.AddRange(_fallingTetrominoes.Where(x => x.BoxAmount == 0));
        emptyTetrominoes.AddRange(_staticTetrominoes .Where(x => x.BoxAmount == 0));

        foreach(var emptyTetromino in emptyTetrominoes)
        {
            DestroyTetromino(emptyTetromino);
        }
    }

    private void DestroyTetromino(Tetromino tetromino)
    {
        if (_fallingTetrominoes.Contains(tetromino))
        {
            _fallingTetrominoes.Remove(tetromino);
        }

        if (_staticTetrominoes.Contains(tetromino))
        {
            _fallingTetrominoes.Remove(tetromino);
        }

        if (tetromino == null) { return; }
        Destroy(tetromino.gameObject);
    }

    private void MoveHighlightedTetrominoDown()
    {
        _highlightedTetromino?.AttemptToMoveInDirection(Direction.Down);
    }

    private void MoveActiveTetrominoesDown()
    {
        foreach (var tetromino in _fallingTetrominoes.OrderBy(x => x.transform.position.y))
        {
            // Prevents double ticks on the highlighted tetromino while using short ticks too
            if (tetromino.IsHighlighted && _usingShortTick) { continue;  }

            tetromino.AttemptToMoveInDirection(Direction.Down);
        }
    }

    private void IncreasePoolCapacity(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            var newBox= Instantiate(_boxPrefab);
            newBox.Initialize(this);
            AddBoxToPool(newBox);
        }
    }

    private void OnDisable()
    {
        InputManager.OnPlayerInput -= HandlePlayerInput;
    }
}
