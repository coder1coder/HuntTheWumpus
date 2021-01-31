namespace HuntTheWumpus.Model
{
    abstract class Entity
    {
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public string Symbol { get; internal set; }

        public Entity()
        {
            PositionX = 0;
            PositionY = 0;
        }

        public void SetPosition(int x, int y)
        {
            PositionX = x;
            PositionY = y;
        }
    }
}
