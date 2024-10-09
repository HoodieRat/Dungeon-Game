using DungeonGame;
using System;

namespace DungeonGame
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.CursorVisible = false; // Hide the cursor for better visuals
            Game game = new Game();
            game.Run();
        }
    }
}