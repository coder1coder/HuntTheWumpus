namespace HuntTheWumpus.Model
{
    abstract class Unit
    {
        public enum Direction { LEFT, UP, RIGHT, DOWN  }

        public Position Position { get; set; }
        public string Symbol { get; internal set; }
        public bool IsAlive { get; set; }

        public virtual void Collision(Unit unit)
        {
            //do something
        }

        public Unit()
        {
            Position = new Position();
            IsAlive = true;
        }
    }
}
