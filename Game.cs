using HuntTheWumpus.Model;
using System;

namespace HuntTheWumpus
{
    internal class Game
    {
        private readonly Random _rand = new Random();
        private Map Map { get; set; }
        private Player Player { get; set; }
        private Wumpus Wumpus { get; set; }

        private bool ShowEnemies { get; set; }

        private Log Log { get; set; } = new Log();
        private int Steps { get; set; }
        public bool IsGameOver => !Player.IsAlive || !Wumpus.IsAlive;

        public Game(byte height = 9, byte width = 9, byte batsCount = 2, byte holesCount = 2, bool showEnemies = false)
        {
            ShowEnemies = showEnemies;

            Map = new Map(width, height);

            Player = (Player)Map.MoveUnit(new Player(), 
                (byte)Math.Ceiling((double)Map.Size.Width / 2), 
                (byte)Math.Ceiling((double)Map.Size.Height / 2));

            int randX, randY;

            //generate wumpus on map
            do
            {
                randX = _rand.Next(0, Map.Size.Width - 1);
                randY = _rand.Next(0, Map.Size.Height - 1);
            }
            while (
                !(
                //wumpus inside map width/height
                randX >=0 && randX < Map.Size.Width && randY >= 0 && randY < Map.Size.Height
                &&
                //not player position
                (randX < (Player.Position.X - 1) || randX > (Player.Position.X + 1))
                &&
                (randY < (Player.Position.Y - 1) || randY > (Player.Position.Y + 1))
            ));

            randX = 0;
            randY = 0;

            Log.Add("Wumpus generated at " + randX + "," + randY);
            Wumpus = (Wumpus)Map.MoveUnit(new Wumpus(), randX, randY);

            //generate bats on map
            for (int i = 0; i < batsCount; i++)
            {
                do
                {
                    randX = _rand.Next(0, Map.Size.Width - 1);
                    randY = _rand.Next(0, Map.Size.Height - 1);
                }
                while (Map.GetUnitAtPosition(randX, randY) != null);

                Map.MoveUnit(new Bat(), randX, randY);
            }

            //generate holes on map
            for (int i = 0; i < holesCount; i++)
            {
                do
                {
                    randX = _rand.Next(0, Map.Size.Width - 1);
                    randY = _rand.Next(0, Map.Size.Height - 1);
                }
                while (Map.GetUnitAtPosition(randX, randY) != null);

                Map.MoveUnit(new Hole(), randX, randY);
            }
        }
        
        public void Start()
        {
            Steps = 1;

            ConsoleKeyInfo input;

            do
            {
                Log.Add("====\tРаунд: " + Steps + "\t====");
                Render();

                input = Console.ReadKey(true);

                if (input.Key == ConsoleKey.LeftArrow || input.Key == ConsoleKey.RightArrow 
                    || input.Key == ConsoleKey.UpArrow || input.Key == ConsoleKey.DownArrow
                    || input.Key == ConsoleKey.W || input.Key == ConsoleKey.A
                    || input.Key == ConsoleKey.S || input.Key == ConsoleKey.D
                    )
                {
                    PlayerPressKey(input);
                    WumpusGo();
                    Steps++;
                }
            }
            while (input.Key != ConsoleKey.Escape &&  Player.IsAlive && Wumpus.IsAlive);

            Console.Clear();
            if (IsGameOver)
                Console.WriteLine("Игра окончена. Выиграл: " + (Player.IsAlive ? "Игрок" : "Wumpus") + ". Затрачено ходов: " + Steps);

            Console.WriteLine("Нажмите любую клавишу для выхода..");
            Console.ReadKey();
        }
        public void Render()
        {
            Console.Clear();

            //Checking because info player after game started
            ShowMessageIfNearDanger();

            for (int y = 0; y < Map.Size.Height; y++)
            {
                for (int x = 0; x < Map.Size.Width; x++)
                {
                    string content = " ";

                    var unit = Map.GetUnitAtPosition(x, y);

                    if (unit != null)
                    {
                        content = unit.Symbol;

                        if (!ShowEnemies && !(unit is Player))
                            content = " ";
                    }

                    Console.Write('['+ content + ']');
                }
                Console.WriteLine();
            }
            Console.WriteLine("\r\n" + Log);
            Console.SetCursorPosition(0, 0);
        }
        
        private void ShowMessageIfNearDanger()
        {
            for (int y = Player.Position.Y - 1; y < Player.Position.Y + 2; y++)
                for (int x = Player.Position.X - 1; x < Player.Position.X + 2; x++)
                {
                    if (y >= 0 && x >=0 && y < Map.Size.Height && x < Map.Size.Width)
                    {
                        if (Map.GetUnitAtPosition(x,y) != null)
                        {
                            switch (Map.GetUnitAtPosition(x, y))
                            {
                                case Wumpus wumpus:
                                    Log.Add("Вы чувствуете вонь");
                                    break;
                                case Bat bat:
                                    Log.Add("Вы чувствуете шелест");
                                    break;
                                case Hole hole:
                                    Log.Add("Вы чувствуете сквозняк");
                                    break;
                            }
                        }
                    }
                }
        }

        //may be has a bag, dont go to down
        //fucking randomizer
        private void WumpusGo()
        {
            //Adding possible positions
            var positions = new Position[] {
                new Position(Wumpus.Position.X - 1, Wumpus.Position.Y),
                new Position(Wumpus.Position.X + 1, Wumpus.Position.Y),
                new Position(Wumpus.Position.X, Wumpus.Position.Y - 1),
                new Position(Wumpus.Position.X, Wumpus.Position.Y + 1),
            };

            //Excluding bad positions
            var filteredPositions = new Position[] { };
            for (int i = 0; i < 4; i++)
            {
                var canUsePosition =
                    positions[i].X > 0 && positions[i].X < Map.Size.Width
                    &&
                    positions[i].Y > 0 && positions[i].Y < Map.Size.Height
                    &&
                    (Map.GetUnitAtPosition(positions[i]) == null || Map.GetUnitAtPosition(positions[i]) is Player);

                if (canUsePosition)
                {
                    Array.Resize(ref filteredPositions, filteredPositions.Length + 1);
                    filteredPositions[filteredPositions.Length - 1] = positions[i];
                }
            }

            var max = filteredPositions.Length - 1;

            var randIdx = _rand.Next(0, max);// - bag

            Wumpus = (Wumpus)Map.MoveUnit(Wumpus, filteredPositions[randIdx]);

            Player.IsAlive = !(Wumpus.Position.X == Player.Position.X && Wumpus.Position.Y == Player.Position.Y);
        }
        private void PlayerPressKey(ConsoleKeyInfo input)
        {
            switch (input.Key)
            {
                case (ConsoleKey.UpArrow):
                    Player = (Player)Map.MoveUnit(Player, Unit.Direction.UP);
                    break;
                case (ConsoleKey.DownArrow):
                    Player = (Player)Map.MoveUnit(Player, Unit.Direction.DOWN);
                    break;
                case (ConsoleKey.LeftArrow):
                    Player = (Player)Map.MoveUnit(Player, Unit.Direction.LEFT);
                    break;
                case (ConsoleKey.RightArrow):
                    Player = (Player)Map.MoveUnit(Player, Unit.Direction.RIGHT);
                    break;
                case (ConsoleKey.W):
                    Wumpus.IsAlive = Wumpus.Position.Y != Player.Position.Y - 1;
                    break;
                case (ConsoleKey.S):
                    Wumpus.IsAlive = Wumpus.Position.Y != Player.Position.Y + 1;
                    break;
                case (ConsoleKey.A):
                    Wumpus.IsAlive = Wumpus.Position.X != Player.Position.X - 1;
                    break;
                case (ConsoleKey.D):
                    Wumpus.IsAlive = Wumpus.Position.X != Player.Position.X + 1;
                    break;
            }

            Player.IsAlive = !(Wumpus.Position.X == Player.Position.X && Wumpus.Position.Y == Player.Position.Y);
        }
    }
}