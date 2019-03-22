using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Box : MonoBehaviour
{
    [SerializeField] private GameObject _outlineObject;

    private Transform _gameBoardTransform;

    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        _gameBoardTransform = FindObjectOfType<GameBoard>().transform;
    }

    internal void Activate(Transform parentTetromino, Vector2 localPosition, Color boxColor)
    {
        transform.SetParent(parentTetromino);
        transform.localPosition = localPosition;
        _spriteRenderer.color = boxColor;
        gameObject.SetActive(true);
    }

    internal void Deactivate()
    {
        transform.SetParent(_gameBoardTransform);
        gameObject.SetActive(false);
    }

    internal void HighlightBox(bool highlight)
    {
        _outlineObject.SetActive(highlight);
        _spriteRenderer.sortingLayerName = highlight ? "Selected Box" : "Default";
    }
}
