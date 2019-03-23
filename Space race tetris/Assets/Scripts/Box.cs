using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Box : MonoBehaviour
{
    [SerializeField] private GameObject _outlineObject;

    private GameBoard _gameBoard;

    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        _gameBoard = FindObjectOfType<GameBoard>();
    }

    internal bool CanMoveDown
        => !_gameBoard.TileIsOccupied(new Vector2Int((int) transform.position.x, (int) transform.position.y - 1));

    internal void Activate(Transform parentTetromino, Vector2 localPosition, Color boxColor)
    {
        transform.SetParent(parentTetromino);
        transform.localPosition = localPosition;
        _spriteRenderer.color = boxColor;
        gameObject.SetActive(true);
    }

    internal void Deactivate()
    {
        if (_gameBoard == null) { _gameBoard = FindObjectOfType<GameBoard>(); }

        transform.SetParent(_gameBoard.transform);
        gameObject.SetActive(false);
    }

    internal void HighlightBox(bool highlight)
    {
        _outlineObject.SetActive(highlight);
        _spriteRenderer.sortingLayerName = highlight ? "Selected Box" : "Default";
    }
}
