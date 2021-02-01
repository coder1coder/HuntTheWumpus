﻿using HuntTheWumpus.Model;

namespace ConsoleGame.Model
{
    class Map
    {
        private readonly Unit[,] _map;

        public MapSize Size { get; private set; }

        public Map(byte width, byte height)
        {
            _map = new Unit[(height < 9) ? 9 : height, (width < 9) ? 9 : width];
            Size = new MapSize((byte)_map.GetLength(1), (byte)_map.GetLength(0));
        }

        internal void ClearUnitAtPosition(Position position)
        {
            ClearUnitAtPosition(position.X, position.Y);
        }

        internal void ClearUnitAtPosition(int x, int y)
        {
            _map[y, x] = null;
        }

        internal Unit GetUnitAtPosition(int x, int y)
        {
            return _map[y, x] ?? null;
        }

        internal Unit GetUnitAtPosition(Position position)
        {
            return GetUnitAtPosition(position.X, position.Y);
        }

        internal Unit MoveUnit(Unit unit, Unit.Direction direction)
        {
            ClearUnitAtPosition(unit.Position);

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

            _map[unit.Position.Y, unit.Position.X] = unit;
            return unit;
        }
        internal Unit MoveUnit(Unit unit, Position position)
        {
            ClearUnitAtPosition(unit.Position);
            unit.Position = position;

            _map[unit.Position.Y, unit.Position.X] = unit;
            return unit;
        }
        internal Unit MoveUnit(Unit unit, int x, int y)
        {
            ClearUnitAtPosition(unit.Position.X, unit.Position.Y);
            unit.Position.X = x;
            unit.Position.Y = y;
            _map[y, x] = unit;
            return unit;
        }
    }
}
