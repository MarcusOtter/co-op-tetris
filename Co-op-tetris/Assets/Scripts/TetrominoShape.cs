public struct TetrominoShape
{
    internal readonly char Letter;
    internal readonly char[,] Shape;
    internal readonly int Rotation;

    public TetrominoShape(char letter, char[,] shape, int rotation)
    {
        Letter = letter;
        Shape = shape;
        Rotation = rotation;
    }

    public static bool operator ==(TetrominoShape shape1, TetrominoShape shape2)
    {
        return shape1.Equals(shape2);
    }

    public static bool operator !=(TetrominoShape shape1, TetrominoShape shape2)
    {
        return !shape1.Equals(shape2);
    }

    public override bool Equals(object obj)
    {
        if (!(obj is TetrominoShape)) { return false; }
        var otherTetrominoShape = (TetrominoShape) obj;

        return Letter == otherTetrominoShape.Letter
            && Rotation == otherTetrominoShape.Rotation;
    }

    public override int GetHashCode()
    {
        return Letter.GetHashCode() ^ Rotation.GetHashCode();
    }
}
