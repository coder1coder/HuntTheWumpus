using HuntTheWumpus.Model;

namespace ConsoleGame.Model
{
    class Map
    {
        private object[,] _map;

        public Map(byte width, byte height)
        {
            _map = new object[(height < 9) ? 9 : height, (width < 9) ? 9 : width];
        }

        public void SetUnitAtPosition(Unit unit, Position position)
        {
            _map[position.Y, position.X] = unit;
        }

        internal void ClearUnitAtPosition(Position position)
        {
            _map[position.Y, position.X] = null;
        }

        internal void ClearUnitAtPosition(byte x, byte y)
        {
            _map[y, x] = null;
        }

        public Unit GetUnitAtPosition(byte x, byte y)
        {

        }
    }
}
