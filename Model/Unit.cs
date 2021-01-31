﻿using ConsoleGame.Model;

namespace HuntTheWumpus.Model
{
    abstract class Unit
    {
        public Position Position { get; set; }
        public string Symbol { get; internal set; }

        public Unit()
        {
            Position = new Position(0, 0);
        }
    }
}
