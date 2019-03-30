using System.Collections.Generic;

public static class TetrominoShapeHelper
{
    private static readonly System.Random _random = new System.Random();

    internal static TetrominoShape GetRandomTetrominoShape()
    {
        var letter = GetRandomLetter();
        var rotation = GetRandomRotation();
        var shape = GetShape(letter, rotation);

        return new TetrominoShape(letter, shape, rotation);
    }

    internal static TetrominoShape GetTetrominoShape(char letter, int rotation)
    {
        return new TetrominoShape(letter, GetShape(letter, rotation), rotation);
    }

    internal static TetrominoShape RotateShape(TetrominoShape shapeToRotate, int rotationDelta)
    {
        int oldRotation = shapeToRotate.Rotation;
        char oldLetter = shapeToRotate.Letter;

        int newRotation = GetValidRotation(oldRotation + rotationDelta);
        char[,] newShape = GetShape(oldLetter, newRotation);

        return new TetrominoShape(oldLetter, newShape, newRotation);
    }
    
    private static int GetValidRotation(int rotation)
    {
        switch (rotation)
        {
            case -360: return 0;
            case -270: return 90;
            case -180: return 180;
            case -90:  return 270;
            case 0:    return 0;
            case 90:   return 90;
            case 180:  return 180;
            case 270:  return 270;
            case 360:  return 0;
            case 450:  return 90;
            case 540:  return 180;
            case 630:  return 270;
            
            default: throw new System.Exception("Invalid rotation. Don't rotate more than 360 degrees at once.");
        }
    }

    private static char[,] GetShape(char letter, int rotation)
    {
        return _allShapes[(letter, rotation)];
    }

    private static int GetRandomRotation()
    {
        return _availableRotations[_random.Next(0, _availableRotations.Length)];
    }

    private static char GetRandomLetter()
    {
        return _availableLetters[_random.Next(0, _availableLetters.Length)];
    }

    private static readonly Dictionary<(char, int), char[,]> _allShapes = new Dictionary<(char, int), char[,]>
    {
        #region Shape: I
        {('I', 0), new char[,]
        {
            { ' ', 'C', ' ', ' ' },
            { ' ', 'C', ' ', ' ' },
            { ' ', 'C', ' ', ' ' },
            { ' ', 'C', ' ', ' ' }
        }},
        {('I', 90), new char[,]
        {
            { ' ', ' ', ' ', ' ' },
            { ' ', ' ', ' ', ' ' },
            { ' ', ' ', ' ', ' ' },
            { 'C', 'C', 'C', 'C' }
        }},
        {('I', 180), new char[,]
        {
            { ' ', 'C', ' ', ' ' },
            { ' ', 'C', ' ', ' ' },
            { ' ', 'C', ' ', ' ' },
            { ' ', 'C', ' ', ' ' }
        }},
        {('I', 270), new char[,]
        {
            { ' ', ' ', ' ', ' ' },
            { ' ', ' ', ' ', ' ' },
            { ' ', ' ', ' ', ' ' },
            { 'C', 'C', 'C', 'C' }
        }},
        #endregion

        #region Shape: J
        {('J', 0), new char[,]
        {
            { ' ', ' ', ' ', ' ' },
            { ' ', ' ', 'B', ' ' },
            { ' ', ' ', 'B', ' ' },
            { ' ', 'B', 'B', ' ' }
        }},
        {('J', 90), new char[,]
        {
            { ' ', ' ', ' ', ' ' },
            { ' ', ' ', ' ', ' ' },
            { 'B', ' ', ' ', ' ' },
            { 'B', 'B', 'B', ' ' }
        }},
        {('J', 180), new char[,]
        {
            { ' ', ' ', ' ', ' ' },
            { ' ', 'B', 'B', ' ' },
            { ' ', 'B', ' ', ' ' },
            { ' ', 'B', ' ', ' ' }
        }},
        {('J', 270), new char[,]
        {
            { ' ', ' ', ' ', ' ' },
            { ' ', ' ', ' ', ' ' },
            { 'B', 'B', 'B', ' ' },
            { ' ', ' ', 'B', ' ' }
        }},
        #endregion

        #region Shape: L
        {('L', 0), new char[,]
        {
            { ' ', ' ', ' ', ' ' },
            { ' ', 'O', ' ', ' ' },
            { ' ', 'O', ' ', ' ' },
            { ' ', 'O', 'O', ' ' }
        }},
        {('L', 90), new char[,]
        {
            { ' ', ' ', ' ', ' ' },
            { ' ', ' ', ' ', ' ' },
            { 'O', 'O', 'O', ' ' },
            { 'O', ' ', ' ', ' ' }
        }},
        {('L', 180), new char[,]
        {
            { ' ', ' ', ' ', ' ' },
            { ' ', 'O', 'O', ' ' },
            { ' ', ' ', 'O', ' ' },
            { ' ', ' ', 'O', ' ' }
        }},
        {('L', 270), new char[,]
        {
            { ' ', ' ', ' ', ' ' },
            { ' ', ' ', ' ', ' ' },
            { ' ', ' ', 'O', ' ' },
            { 'O', 'O', 'O', ' ' }
        }},
        #endregion

        #region Shape: O
        {('O', 0), new char[,]
        {
            { ' ', ' ', ' ', ' ' },
            { ' ', ' ', ' ', ' ' },
            { ' ', 'Y', 'Y', ' ' },
            { ' ', 'Y', 'Y', ' ' }
        }},
        {('O', 90), new char[,]
        {
            { ' ', ' ', ' ', ' ' },
            { ' ', ' ', ' ', ' ' },
            { ' ', 'Y', 'Y', ' ' },
            { ' ', 'Y', 'Y', ' ' }
        }},
        {('O', 180), new char[,]
        {
            { ' ', ' ', ' ', ' ' },
            { ' ', ' ', ' ', ' ' },
            { ' ', 'Y', 'Y', ' ' },
            { ' ', 'Y', 'Y', ' ' }
        }},
        {('O', 270), new char[,]
        {
            { ' ', ' ', ' ', ' ' },
            { ' ', ' ', ' ', ' ' },
            { ' ', 'Y', 'Y', ' ' },
            { ' ', 'Y', 'Y', ' ' }
        }},
        #endregion

        #region Shape: S
        {('S', 0), new char[,]
        {
            { ' ', ' ', ' ', ' ' },
            { ' ', ' ', ' ', ' ' },
            { ' ', 'G', 'G', ' ' },
            { 'G', 'G', ' ', ' ' }
        }},
        {('S', 90), new char[,]
        {
            { ' ', ' ', ' ', ' ' },
            { ' ', 'G', ' ', ' ' },
            { ' ', 'G', 'G', ' ' },
            { ' ', ' ', 'G', ' ' }
        }},
        {('S', 180), new char[,]
        {
            { ' ', ' ', ' ', ' ' },
            { ' ', ' ', ' ', ' ' },
            { ' ', 'G', 'G', ' ' },
            { 'G', 'G', ' ', ' ' }
        }},
        {('S', 270), new char[,]
        {
            { ' ', ' ', ' ', ' ' },
            { ' ', 'G', ' ', ' ' },
            { ' ', 'G', 'G', ' ' },
            { ' ', ' ', 'G', ' ' }
        }},
        #endregion

        #region Shape: Z
        {('Z', 0), new char[,]
        {
            { ' ', ' ', ' ', ' ' },
            { ' ', ' ', ' ', ' ' },
            { 'R', 'R', ' ', ' ' },
            { ' ', 'R', 'R', ' ' }
        }},
        {('Z', 90), new char[,]
        {
            { ' ', ' ', ' ', ' ' },
            { ' ', ' ', 'R', ' ' },
            { ' ', 'R', 'R', ' ' },
            { ' ', 'R', ' ', ' ' }
        }},
        {('Z', 180), new char[,]
        {
            { ' ', ' ', ' ', ' ' },
            { ' ', ' ', ' ', ' ' },
            { 'R', 'R', ' ', ' ' },
            { ' ', 'R', 'R', ' ' }
        }},
        {('Z', 270), new char[,]
        {
            { ' ', ' ', ' ', ' ' },
            { ' ', ' ', 'R', ' ' },
            { ' ', 'R', 'R', ' ' },
            { ' ', 'R', ' ', ' ' }
        }},
        #endregion

        #region Shape: T
        {('T', 0), new char[,]
        {
            { ' ', ' ', ' ', ' ' },
            { ' ', ' ', ' ', ' ' },
            { 'P', 'P', 'P', ' ' },
            { ' ', 'P', ' ', ' ' }
        }},
        {('T', 90), new char[,]
        {
            { ' ', ' ', ' ', ' ' },
            { ' ', ' ', 'P', ' ' },
            { ' ', 'P', 'P', ' ' },
            { ' ', ' ', 'P', ' ' }
        }},
        {('T', 180), new char[,]
        {
            { ' ', ' ', ' ', ' ' },
            { ' ', ' ', ' ', ' ' },
            { 'P', 'P', 'P', ' ' },
            { ' ', 'P', ' ', ' ' }
        }},
        {('T', 270), new char[,]
        {
            { ' ', ' ', ' ', ' ' },
            { ' ', ' ', 'P', ' ' },
            { ' ', 'P', 'P', ' ' },
            { ' ', ' ', 'P', ' ' }
        }},
        #endregion
    };
    private static readonly char[] _availableLetters = new char[7] { 'I', 'J', 'L', 'O', 'S', 'Z', 'T' };
    private static readonly int[] _availableRotations = new int[4] { 0, 90, 180, 270 };
}
