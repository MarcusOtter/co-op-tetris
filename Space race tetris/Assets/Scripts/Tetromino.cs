using System.Collections;
using UnityEngine;

public class Tetromino : MonoBehaviour
{
    private TetrominoShape _tetrominoShape;

    private BoxPool _boxPool;

    private void Awake()
    {
        _tetrominoShape = new TetrominoShape();
        //print($"We got a {_tetrominoShape.Letter} tetromino");
    }

    private void Start()
    {
        _boxPool = FindObjectOfType<BoxPool>();
        StartCoroutine(TempGenerator());
    }

    private IEnumerator TempGenerator()
    {
        while (true)
        {
            RemoveAllChildren();

            TetrominoShape newShape;
            do
            {
                newShape = new TetrominoShape();
            }
            while (_tetrominoShape == newShape);

            _tetrominoShape = newShape;
            DrawTetromino();
            yield return new WaitForSeconds(0.25f);
        }
    }

    private void RemoveAllChildren()
    {
        while (transform.childCount != 0)
        {
            _boxPool.AddBoxToPool(transform.GetChild(0).gameObject);
        }
    }

    private void DrawTetromino()
    {
        for (int y = 0; y < 4; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                var letter = _tetrominoShape.Shape[y, x];
                if (letter == ' ') { continue; }

                var box = _boxPool.GetBox();
                box.transform.SetParent(transform);
                box.transform.localPosition = new Vector2(x, y);
                box.GetComponent<SpriteRenderer>().color = GetColorFromLetter(letter);
                box.gameObject.SetActive(true);
            }
        }
    }

    private Color GetColorFromLetter(char letter)
    {
        switch (letter)
        {
            case 'C': return Color.cyan;
            case 'B': return Color.blue;
            case 'O': return new Color32(232, 157, 38, 255); // Orange
            case 'Y': return Color.yellow;
            case 'G': return Color.green;
            case 'R': return Color.red;
            case 'P': return new Color32(120, 30, 237, 255); // Purple

            default: throw new System.Exception($"There is no color that corresponds with the letter '{letter}'");
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
