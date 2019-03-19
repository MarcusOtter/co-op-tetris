using System;

public static class TetrominoHelper
{
    private static Random _random;

    static TetrominoHelpers()
    {
        _random = new Random();
    }

    public static char[,] GetRandomTetronimoShape()
    {
        var randomNum = _random.Next(0, 7);
        switch(randomNum)
        {
            case 0: return I;
            case 1: return J;
            case 2: return L;
            case 3: return O;
            case 4: return S;
            case 5: return Z;
            case 6: return T;

            default:
                throw new Exception("Tetromino not found");
        }
    }

    public readonly static char[,] I =
    {
        { ' ', ' ', 'C', ' ' },
        { ' ', ' ', 'C', ' ' },
        { ' ', ' ', 'C', ' ' },
        { ' ', ' ', 'C', ' ' }
    };

    public readonly static char[,] J =
    {
        { ' ', ' ', 'B', ' ' },
        { ' ', ' ', 'B', ' ' },
        { ' ', 'B', 'B', ' ' },
        { ' ', ' ', ' ', ' ' }
    };

    public readonly static char[,] L =
    {
        { ' ', 'O', ' ', ' ' },
        { ' ', 'O', ' ', ' ' },
        { ' ', 'O', 'O', ' ' },
        { ' ', ' ', ' ', ' ' }
    };

    public readonly static char[,] O =
    {
        { ' ', ' ', ' ', ' ' },
        { ' ', 'Y', 'Y', ' ' },
        { ' ', 'Y', 'Y', ' ' },
        { ' ', ' ', ' ', ' ' }
    };

    public readonly static char[,] S =
    {
        { ' ', ' ', ' ', ' ' },
        { ' ', ' ', 'G', 'G' },
        { ' ', 'G', 'G', ' ' },
        { ' ', ' ', ' ', ' ' }
    };

    public readonly static char[,] Z =
    {
        { ' ', ' ', ' ', ' ' },
        { 'R', 'R', ' ', ' ' },
        { ' ', 'R', 'R', ' ' },
        { ' ', ' ', ' ', ' ' }
    };

    public readonly static char[,] T =
    {
        { ' ', ' ', ' ', ' ' },
        { ' ', 'P', 'P', 'P' },
        { ' ', ' ', 'P', ' ' },
        { ' ', ' ', ' ', ' ' }
    };
}
