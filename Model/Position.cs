namespace ConsoleGame.Model
{
    class Position
    {
        public byte X { get; set; }
        public byte Y { get; set; }

        public  Position(byte x, byte y)
        {
            X = x;
            Y = y;
        }

        public void Set(byte x, byte y)
        {
            X = x;
            Y = y;
        }
    }
}
