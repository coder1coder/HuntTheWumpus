namespace HuntTheWumpus.Model
{
    internal class Player : Unit
    {
        public Player()
        {
            Symbol = "@";
        }

        public void MoveUp() => PositionY -= 1;
        public void MoveDown() => PositionY += 1;
        public void MoveLeft() => PositionX -= 1;
        public void MoveRight() => PositionX += 1;
    }
}