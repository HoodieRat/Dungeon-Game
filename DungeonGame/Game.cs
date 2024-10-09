using System;
using System.Collections.Generic;
using System.Threading;

namespace DungeonGame
{
    class Game
    {
        private Map map;
        private Player player;
        private bool gameRunning = true;

        public void Run()
        {
            ShowMenu();
            while (gameRunning)
            {
                RenderView();
                HandleInput();
                CheckGameOver();
                Thread.Sleep(50); // Small delay to reduce CPU usage
            }
            Console.CursorVisible = true; // Show the cursor when exiting
        }

        private void ShowMenu()
        {
            Console.Clear();
            Console.WriteLine("=== ASCII Dungeon Crawler ===");
            Console.WriteLine("1. Start New Game");
            Console.WriteLine("2. Load Game with Seed");
            Console.WriteLine("3. Exit");
            Console.Write("Choose an option: ");
            string choice = Console.ReadLine();

            int seed = 0;
            switch (choice)
            {
                case "1":
                    seed = new Random().Next();
                    break;
                case "2":
                    Console.Write("Enter seed: ");
                    if (!int.TryParse(Console.ReadLine(), out seed))
                    {
                        Console.WriteLine("Invalid seed. Starting with a random seed.");
                        seed = new Random().Next();
                    }
                    break;
                case "3":
                    gameRunning = false;
                    return;
                default:
                    Console.WriteLine("Invalid option. Starting new game.");
                    seed = new Random().Next();
                    break;
            }

            map = new Map(seed);
            player = new Player(map.StartX, map.StartY);
        }

        private void RenderView()
        {
            var (buffer, colorBuffer) = Renderer.Render(map, player);
            Console.SetCursorPosition(0, 0);
            for (int y = 0; y < Renderer.ViewHeight; y++)
            {
                for (int x = 0; x < Renderer.ViewWidth; x++)
                {
                    Console.ForegroundColor = colorBuffer[y, x];
                    Console.Write(buffer[y, x]);
                }
                Console.WriteLine();
            }
            Console.ResetColor();
            Console.WriteLine($"Position: ({player.X}, {player.Y})  Direction: {player.FacingDirection}");
            Console.WriteLine($"Keys: {player.Keys}");
            Console.WriteLine("Controls: W=Forward, S=Backward, A=Rotate Left, D=Rotate Right, Q=Strafe Left, E=Strafe Right, Space=Interact, Esc=Exit");
        }

        private void HandleInput()
        {
            while (Console.KeyAvailable)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                switch (keyInfo.Key)
                {
                    case ConsoleKey.W:
                        player.MoveForward(map);
                        break;
                    case ConsoleKey.S:
                        player.MoveBackward(map);
                        break;
                    case ConsoleKey.A:
                        player.RotateLeft();
                        break;
                    case ConsoleKey.D:
                        player.RotateRight();
                        break;
                    case ConsoleKey.Q:
                        if (!player.IsSpaceRestricted(map))
                            player.StrafeLeft(map);
                        break;
                    case ConsoleKey.E:
                        if (!player.IsSpaceRestricted(map))
                            player.StrafeRight(map);
                        break;
                    case ConsoleKey.Spacebar:
                        player.Interact(map);
                        break;
                    case ConsoleKey.Escape:
                        gameRunning = false;
                        break;
                }
            }
        }

        private void CheckGameOver()
        {
            // Add any game over conditions here, e.g., reaching an exit.
        }
    }
}