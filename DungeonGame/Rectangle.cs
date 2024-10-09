namespace DungeonGame
{
    struct Rectangle
    {
        public int X1, Y1, X2, Y2;
        public int CenterX => (X1 + X2) / 2;
        public int CenterY => (Y1 + Y2) / 2;

        public Rectangle(int x, int y, int width, int height)
        {
            X1 = x;
            Y1 = y;
            X2 = x + width - 1;
            Y2 = y + height - 1;
        }

        public bool Intersects(Rectangle other)
        {
            return X1 <= other.X2 && X2 >= other.X1 &&
                   Y1 <= other.Y2 && Y2 >= other.Y1;
        }
    }
}