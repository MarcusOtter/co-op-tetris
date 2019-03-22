using System;

public class TetrominoShape
{
    internal readonly char Letter;
    internal readonly char[,] Shape;
    // Rotation variable or make the Shape non-readonly

    private readonly char[] _availableLetters 
        = new char[] { 'I', 'J', 'L', 'O', 'S', 'Z', 'T' };

    private static Random _random;

    /// <summary>
    /// Constructs a random shape
    /// </summary>
    public TetrominoShape()
    {
        if (_random == null) { _random = new Random(); }

        var randomLetter = _availableLetters[_random.Next(0, _availableLetters.Length)];

        Letter = randomLetter;
        Shape = GetShapeByLetter(randomLetter);
    }

    /// <summary>
    /// Constructs a shape from the given letter
    /// </summary>
    /// <param name="letter">The letter to use as a tetromino shape</param>
    public TetrominoShape(char letter)
    {
        Letter = letter;
        Shape = GetShapeByLetter(letter);
    }
    
    private char[,] GetShapeByLetter(char letter)
    {
        switch (char.ToUpper(letter))
        {
            case 'I': return _shape_I;
            case 'J': return _shape_J;
            case 'L': return _shape_L;
            case 'O': return _shape_O;
            case 'S': return _shape_S;
            case 'Z': return _shape_Z;
            case 'T': return _shape_T;

            default: throw new Exception($"There is no '{letter}' tetromino.");
        }
    }

    private readonly char[,] _shape_I =
    {
        { ' ', ' ', 'C', ' ' },
        { ' ', ' ', 'C', ' ' },
        { ' ', ' ', 'C', ' ' },
        { ' ', ' ', 'C', ' ' }
    };

    private readonly char[,] _shape_J =
    {
        { ' ', ' ', 'B', ' ' },
        { ' ', ' ', 'B', ' ' },
        { ' ', 'B', 'B', ' ' },
        { ' ', ' ', ' ', ' ' }
    };

    private readonly char[,] _shape_L =
    {
        { ' ', 'O', ' ', ' ' },
        { ' ', 'O', ' ', ' ' },
        { ' ', 'O', 'O', ' ' },
        { ' ', ' ', ' ', ' ' }
    };

    private readonly char[,] _shape_O =
    {
        { ' ', ' ', ' ', ' ' },
        { ' ', 'Y', 'Y', ' ' },
        { ' ', 'Y', 'Y', ' ' },
        { ' ', ' ', ' ', ' ' }
    };

    private readonly char[,] _shape_S =
    {
        { ' ', ' ', ' ', ' ' },
        { ' ', ' ', 'G', 'G' },
        { ' ', 'G', 'G', ' ' },
        { ' ', ' ', ' ', ' ' }
    };

    private readonly char[,] _shape_Z =
    {
        { ' ', ' ', ' ', ' ' },
        { 'R', 'R', ' ', ' ' },
        { ' ', 'R', 'R', ' ' },
        { ' ', ' ', ' ', ' ' }
    };

    private readonly char[,] _shape_T =
    {
        { ' ', ' ', ' ', ' ' },
        { ' ', 'P', 'P', 'P' },
        { ' ', ' ', 'P', ' ' },
        { ' ', ' ', ' ', ' ' }
    };
}
