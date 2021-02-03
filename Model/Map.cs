using System;

namespace HuntTheWumpus.Model
{
    class Map
    {
        private readonly Unit[,] _map;

        public MapSize Size => new MapSize((byte)_map.GetLength(1), (byte)_map.GetLength(0));

        public Map(byte width, byte height)
        {
            //minimum size 9x9
            _map = new Unit[(height < 9) ? 9 : height, (width < 9) ? 9 : width];
        }

        internal void RemoveUnit(Position position) => RemoveUnit(position.X, position.Y);
        internal void RemoveUnit(int x, int y) => _map[y, x] = null;

        internal Unit GetUnit(int x, int y) => _map[y, x] ?? null;
        internal Unit GetUnit(Position position) => GetUnit(position.X, position.Y);

        internal Unit MoveUnit(Unit unit, Unit.Direction direction)
        {
            RemoveUnit(unit.Position);

            switch(direction)
            {
                case Unit.Direction.UP:
                    if (unit.Position.Y > 0)
                        unit.Position.Y -= 1;
                    break;
                case Unit.Direction.DOWN:
                    if (unit.Position.Y < Size.Height - 1)
                        unit.Position.Y += 1;
                    break;
                case Unit.Direction.LEFT:
                    if (unit.Position.X > 0)
                        unit.Position.X -= 1;
                    break;
                case Unit.Direction.RIGHT:
                    if (unit.Position.X < Size.Width - 1)
                        unit.Position.X += 1;
                    break;
            }

            return AddUnit(unit, unit.Position);
        }
        internal Unit MoveUnit(Unit unit, int x, int y)
        {
            RemoveUnit(unit.Position.X, unit.Position.Y);
            return AddUnit(unit, x, y);
        }
        internal Unit MoveUnit(Unit unit, Position position) => MoveUnit(unit, position.X, position.Y);

        internal Unit AddUnit(Unit unit, Position position) => AddUnit(unit, position.X, position.Y);
        internal Unit AddUnit(Unit unit, int x, int y)
        {
            unit.Collision(_map[y, x]);

            unit.Position.X = x;
            unit.Position.Y = y;
            //in future may be add validate if unit exist at this position
            _map[y, x] = unit;
            return unit;
        }
    }
}
