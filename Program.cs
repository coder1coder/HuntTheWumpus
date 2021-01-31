using System;
using System.Text;

namespace HuntTheWumpus
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            var game = new Game(showBatsAndHoles: true);
            game.Start();
        }
    }
}
