using ConsoleGame.Model;
using HuntTheWumpus.Model;
using System;

namespace HuntTheWumpus
{
    internal class Game
    {
        private readonly Random Randomizer;
        private ConsoleKeyInfo _input;
        private ConsoleKeyInfo Input
        {
            get => _input;
            set
            {
                if (value != _input)
                    _input = value;

                switch (_input.Key)
                {
                    case (ConsoleKey.UpArrow):
                        if (Player.Position.Y > 0)
                            Player.MoveUp();
                        break;
                    case (ConsoleKey.DownArrow):
                        if (Player.Position.Y < Map.GetLength(0) - 1)
                            Player.MoveDown();
                        break;
                    case (ConsoleKey.LeftArrow):
                        if (Player.Position.X > 0)
                            Player.MoveLeft();
                        break;
                    case (ConsoleKey.RightArrow):
                        if (Player.Position.X < Map.GetLength(1) - 1)
                            Player.MoveRight();
                        break;
                    case (ConsoleKey.Spacebar):

                        if (IsWumpusNear())
                        {
                            AddLog("Wumpus мертв! Вы победили!");
                            IsGameOver = true;
                        } 
                        else AddLog("Выстрел в воздух, какая досада :(");
                        break;
                }
                
                if (!IsGameOver)
                {
                    UpdatePlayerPosition(Player);
                    ValidateDeath();
                    WumpusGo();
                    ValidateDeath();
                }
            }
        }

        private void WumpusGo()
        {
            var lastY = Wumpus.Position.Y;
            var lastX = Wumpus.Position.X;

            bool dorepeat;

            Map[Wumpus.Position.Y, Wumpus.Position.X] = null;

            do
            {
                Wumpus.Position.Set(
                    (byte)Randomizer.Next(Wumpus.Position.X - 1, Wumpus.Position.X + 2),
                    (byte)Randomizer.Next(Wumpus.Position.Y - 1, Wumpus.Position.Y + 2)
                );

                //большие проблемы с множественными условиями, голова уехала, но надо сделать по человечески исключив try catch
                try
                {
                    dorepeat = (Wumpus.Position.Y < 0 || Wumpus.Position.Y >= Map.GetLength(0) || Wumpus.Position.X < 0 || Wumpus.Position.X >= Map.GetLength(0))
                    &&
                    (Map[Wumpus.Position.Y, Wumpus.Position.X] is Bat || Map[Wumpus.Position.Y, Wumpus.Position.X] is Hole)
                    &&
                    Wumpus.Position.X == lastX && Wumpus.Position.Y == lastY;
                }
                catch (IndexOutOfRangeException)
                {
                    dorepeat = true;
                }
                
            }
            while (dorepeat);

            Map[Wumpus.Position.Y, Wumpus.Position.X] = Wumpus;
        }

        private void ValidateDeath()
        {
            switch(Map[Player.Position.Y, Player.Position.X])
            {
                case Wumpus wumpus:
                    AddLog("Wumpuuuuuuuuusss!!!!");
                    IsGameOver = true;
                    break;
                case Bat bat:
                    AddLog("Вас сожрали летучие мыши!");
                    IsGameOver = true;
                    break;
                case Hole hole:
                    AddLog("Вы упали в яму!");
                    IsGameOver = true;
                    break;
            }
        }

        private object[,] Map { get; set; }
        private Player Player { get; set; }
        private Wumpus Wumpus { get; set; }

        private bool ShowEnemies { get; set; }

        private string Log { get; set; }
        private int Steps { get; set; }
        public bool IsGameOver { get; private set; }

        public Game(byte height = 9, byte width = 9, byte batsCount = 2, byte holesCount = 2, bool showEnemies = false)
        {
            ShowEnemies = showEnemies;

            Randomizer = new Random();

            Map = new object[(height < 9) ? 9 : height, (width < 9) ? 9 : width];

            Player = new Player();
            Player.Position.Set(
                (byte)Math.Ceiling((double)Map.GetLength(1) / 2),
                (byte)Math.Ceiling((double)Map.GetLength(0) / 2)
            );

            Map[Player.Position.Y, Player.Position.X] = Player;

            Wumpus = new Wumpus();

            do
            {
                Wumpus.Position.Set((byte)Randomizer.Next(0, Map.GetLength(1) - 1), (byte)Randomizer.Next(0, Map.GetLength(0) - 1));
            }
            while (!(Wumpus.Position.X < (Player.Position.X - 1) || Wumpus.Position.X > (Player.Position.X + 1))
                &&
            !(Wumpus.Position.Y < (Player.Position.Y - 1) || Wumpus.Position.Y > (Player.Position.Y + 1)));

            Map[Wumpus.Position.Y, Wumpus.Position.X] = Wumpus;

            for (int i = 0; i < batsCount; i++)
            {
                var bat = new Bat();

                do
                {
                    bat.Position.X = (byte)Randomizer.Next(0, Map.GetLength(1) - 1);
                    bat.Position.Y = (byte)Randomizer.Next(0, Map.GetLength(0) - 1);
                }
                while (Map[bat.Position.Y, bat.Position.X] != null);

                Map[bat.Position.Y, bat.Position.X] = bat;
            }

            for (int i = 0; i < holesCount; i++)
            {
                var hole = new Hole();
                do
                {
                    hole.Position.X = (byte)Randomizer.Next(0, Map.GetLength(1) - 1);
                    hole.Position.Y = (byte)Randomizer.Next(0, Map.GetLength(0) - 1);
                }
                while (Map[hole.Position.Y, hole.Position.X] != null);

                Map[hole.Position.Y, hole.Position.X] = hole;
            }
        }
        
        public void Start()
        {
            Steps = 1;

            do
            {
                AddLog("====\tРаунд: " + Steps + "\t====");
                Draw();
                Input = Console.ReadKey();
                Steps++;
            }
            while (Input.Key != ConsoleKey.Escape && !IsGameOver);//wtf?why AND, not OR?
            
            if (IsGameOver)
            {
                AddLog("Игра окончена. Затрачено ходов: " + Steps);
                Draw();
            }

            Console.ReadKey();
        }
        public void Draw()
        {
            ShowMessageIfNearDanger();

            Console.Clear();

            for (int i = 0; i < Map.GetLength(0); i++)
            {
                for (int j = 0; j < Map.GetLength(1); j++)
                {
                    string content = " ";
                    if (Map[i,j] != null)
                    {
                        content = ((Unit)Map[i, j]).Symbol;

                        if (!ShowEnemies && !(Map[i, j] is Player))
                            content = " ";
                    }

                    Console.Write('[' + content + ']');
                }
                Console.WriteLine();
            }
            Console.WriteLine("\r\n" + Log);
            Console.SetCursorPosition(0, 0);
        }
        
        private void ShowMessageIfNearDanger()
        {
            for (int h = Player.Position.Y - 1; h < Player.Position.Y + 2; h++)
            {
                for (int w = Player.Position.X - 1; w < Player.Position.X + 2; w++)
                {
                    if (h >= 0 && w >=0 && h < Map.GetLength(1) && w < Map.GetLength(0))
                    {
                        if (Map[h, w] != null)
                        {
                            switch (Map[h, w])
                            {
                                case Wumpus wumpus:
                                    AddLog("Вы чувствуете вонь");
                                    break;
                                case Bat bat:
                                    AddLog("Вы чувствуете шелест");
                                    break;
                                case Hole hole:
                                    AddLog("Вы чувствуете сквозняк");
                                    break;
                            }
                        }
                    }
                }
            }
        }
        private bool IsWumpusNear()
        {
            return 
                Wumpus.Position.Y >= Player.Position.Y - 1 
                && 
                Wumpus.Position.Y <= Player.Position.Y + 1 
                && 
                Wumpus.Position.X >= Player.Position.X - 1 
                && 
                Wumpus.Position.X <= Player.Position.X + 1;

        }
        private void UpdatePlayerPosition(Player player)
        {
            for (int i = 0; i < Map.GetLength(0); i++)
                for (int j = 0; j < Map.GetLength(1); j++)
                {
                    if (Map[i, j] is Player)
                    {
                        Map[i, j] = null;
                        break;
                    }
                }
            Map[player.Position.Y, player.Position.X] = player;
        }

        public void AddLog(string message)
        {
            Log = message + "\r\n" + Log;
        }
    }
}