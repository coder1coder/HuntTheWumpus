namespace HuntTheWumpus.Model
{
    internal class Player : Unit
    {
        public Player()
        {
            Symbol = "@";
        }

        public void Move(Direction direction)
        {
            switch (direction)
            {
                case Direction.UP:
                    Position.Y -= 1;
                    break;
                case Direction.DOWN:
                    Position.Y += 1;
                    break;
                case Direction.LEFT:
                    Position.X -= 1;
                    break;
                case Direction.RIGHT:
                    Position.X += 1;
                    break;
            }
        }

        public void Shot(Direction direction)
        {
            //switch(direction)
            //{
            //    case Direction.UP:
            //        Position.Y -= 1;
            //        break;
            //    case Direction.DOWN:
            //        Position.Y += 1;
            //        break;
            //    case Direction.LEFT:
            //        Position.X -= 1;
            //        break;
            //    case Direction.RIGHT:
            //        Position.X += 1;
            //        break;
            //}
        }
    }
}