namespace HuntTheWumpus.Model
{
    internal class Player : Unit
    {
        public Player()
        {
            Symbol = "@";
        }

        public void MoveUp() => Position.Y -= 1;
        public void MoveDown() => Position.Y += 1;
        public void MoveLeft() => Position.X -= 1;
        public void MoveRight() => Position.X += 1;
    }
}