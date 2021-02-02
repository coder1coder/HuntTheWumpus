namespace HuntTheWumpus.Model
{
    class Position
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Position()
        {
            //X = 0;
            //Y = 0;
        }
        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
