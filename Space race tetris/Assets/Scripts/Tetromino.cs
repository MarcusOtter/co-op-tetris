using System.Collections;
using UnityEngine;

public class Tetromino : MonoBehaviour
{
    [SerializeField] private int _stepDistance = 1;
    [SerializeField] private float _stepDelay = 2f;

    private bool[,] _boxes =
    {
        { false, false, true, false },
        { false, false, true, false },
        { false, false, true, false },
        { false, false, true, false }
    };

    private Transform[] _tetrominoBoxes;
    // private bool _isFalling = true;

    //private TetrominoManager _tetrominoManager;

    private void Awake()
    {
        _tetrominoBoxes = new Transform[transform.childCount];
        for (int i = 0; i < _tetrominoBoxes.Length; i++)
        {
            _tetrominoBoxes[i] = transform.GetChild(i);
            _tetrominoBoxes[i].gameObject.SetActive(false);
        }

        DrawTetromino();
    }

    private void DrawTetromino()
    {
        for (int y = 0; y < 4; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                if (_boxes[y, x]) 
                {
                    _tetrominoBoxes[y].gameObject.SetActive(true);
                    _tetrominoBoxes[y].localPosition = new Vector2(x, y);
                }
            }
        }
    }

    /*
    private void Start()
    {
        _tetrominoManager = FindObjectOfType<TetrominoManager>();
    }

    private void MoveDown()
    {
        foreach (var box in _tetrominoBoxes)
        {
            if (_tetrominoManager.BoxIsOccupied(new Vector2Int((int) box.position.x, (int) box.position.y - _stepDistance))) { return; }
        }

        transform.position = new Vector3(transform.position.x, transform.position.y - _stepDistance, 0);
    }


    private IEnumerator TempSteps()
    {
        while (_isFalling)
        {
            yield return new WaitForSeconds(_stepDelay);

            MoveDown();
        }
    }
    */
}
