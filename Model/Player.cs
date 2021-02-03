namespace HuntTheWumpus.Model
{
    internal class Player : Unit
    {
        public Player()
        {
            Symbol = "@";
        }

        public override void Collision(Unit unit)
        {
            switch (unit)
            {
                case Bat _: 
                case Hole _:
                    IsAlive = false;
                    break;
            }
        }
    }
}