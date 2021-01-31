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
                        if (Player.PositionY > 0)
                            Player.MoveUp();
                        break;
                    case (ConsoleKey.DownArrow):
                        if (Player.PositionY < Map.GetLength(0) - 1)
                            Player.MoveDown();
                        break;
                    case (ConsoleKey.LeftArrow):
                        if (Player.PositionX > 0)
                            Player.MoveLeft();
                        break;
                    case (ConsoleKey.RightArrow):
                        if (Player.PositionX < Map.GetLength(1) - 1)
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
            var lastY = Wumpus.PositionY;
            var lastX = Wumpus.PositionX;

            bool dorepeat;

            Map[Wumpus.PositionY, Wumpus.PositionX] = null;

            do
            {
                Wumpus.PositionY = Randomizer.Next(Wumpus.PositionY - 1, Wumpus.PositionY + 2);
                Wumpus.PositionX = Randomizer.Next(Wumpus.PositionX - 1, Wumpus.PositionX + 2);

                //большие проблемы с множественными условиями, голова уехала, но надо сделать по человечески исключив try catch
                try
                {
                    dorepeat = (Wumpus.PositionY < 0 || Wumpus.PositionY >= Map.GetLength(0) || Wumpus.PositionX < 0 || Wumpus.PositionX >= Map.GetLength(0))
                    &&
                    (Map[Wumpus.PositionY, Wumpus.PositionX] is Bat || Map[Wumpus.PositionY, Wumpus.PositionX] is Hole)
                    &&
                    Wumpus.PositionX == lastX && Wumpus.PositionY == lastY;
                }
                catch (IndexOutOfRangeException)
                {
                    dorepeat = true;
                }
                
            }
            while (dorepeat);

            Map[Wumpus.PositionY, Wumpus.PositionX] = Wumpus;
        }

        private void ValidateDeath()
        {
            switch(Map[Player.PositionY, Player.PositionX])
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

            Player = new Player()
            {
                PositionX = (int)Math.Ceiling((double)Map.GetLength(1) / 2),
                PositionY = (int)Math.Ceiling((double)Map.GetLength(0) / 2)
            };

            Map[Player.PositionY, Player.PositionX] = Player;

            Wumpus = new Wumpus();

            do
            {
                Wumpus.PositionX = Randomizer.Next(0, Map.GetLength(1) - 1);
                Wumpus.PositionY = Randomizer.Next(0, Map.GetLength(0) - 1);
            }
            while (!(Wumpus.PositionX < (Player.PositionX - 1) || Wumpus.PositionX > (Player.PositionX + 1))
                &&
            !(Wumpus.PositionY < (Player.PositionY - 1) || Wumpus.PositionY > (Player.PositionY + 1)));

            Map[Wumpus.PositionY, Wumpus.PositionX] = Wumpus;

            for (int i = 0; i < batsCount; i++)
            {
                var bat = new Bat();

                do
                {
                    bat.PositionX = Randomizer.Next(0, Map.GetLength(1) - 1);
                    bat.PositionY = Randomizer.Next(0, Map.GetLength(0) - 1);
                }
                while (Map[bat.PositionY, bat.PositionX] != null);

                Map[bat.PositionY, bat.PositionX] = bat;
            }

            for (int i = 0; i < holesCount; i++)
            {
                var hole = new Hole();
                do
                {
                    hole.PositionX = Randomizer.Next(0, Map.GetLength(1) - 1);
                    hole.PositionY = Randomizer.Next(0, Map.GetLength(0) - 1);
                }
                while (Map[hole.PositionY, hole.PositionX] != null);

                Map[hole.PositionY, hole.PositionX] = hole;
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
                        content = ((Entity)Map[i, j]).Symbol;

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
            for (int h = Player.PositionY - 1; h < Player.PositionY + 2; h++)
            {
                for (int w = Player.PositionX - 1; w < Player.PositionX + 2; w++)
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
                Wumpus.PositionY >= Player.PositionY - 1 
                && 
                Wumpus.PositionY <= Player.PositionY + 1 
                && 
                Wumpus.PositionX >= Player.PositionX - 1 
                && 
                Wumpus.PositionX <= Player.PositionX + 1;

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
            Map[player.PositionY, player.PositionX] = player;
        }

        public void AddLog(string message)
        {
            Log = message + "\r\n" + Log;
        }
    }
}