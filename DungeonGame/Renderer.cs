using DungeonGame;
using System;
using System.Text;

namespace DungeonGame
{
    class Renderer
    {
        public static int ViewWidth { get; private set; }
        public static int ViewHeight { get; private set; }
        private static char[,] buffer;
        private static ConsoleColor[,] colorBuffer;
        private static int minimapWidth = 20;  // Width of the minimap
        private static int minimapHeight = 10; // Height of the minimap

        public static (char[,], ConsoleColor[,]) Render(Map map, Player player)
        {
            CellType hitCellType = CellType.Empty;
            int wallOrientation = 0; // 0 for NS walls, 1 for EW walls

            // Adjust view size to console window size, reserving space for minimap
            ViewWidth = Console.WindowWidth - minimapWidth;
            ViewHeight = Console.WindowHeight - 6; // Reserve space for HUD and messages
            buffer = new char[ViewHeight, ViewWidth];
            colorBuffer = new ConsoleColor[ViewHeight, ViewWidth];

            ClearBuffer();

            double fov = Math.PI / 3; // 60 degrees
            int numRays = ViewWidth;
            double maxDepth = 20;

            double rayAngleStart = player.Angle - fov / 2;
            double angleStep = fov / numRays;

            double previousDistance = 0;
            ConsoleColor previousColor = ConsoleColor.Black;

            for (int x = 0; x < numRays; x++)
            {
                double rayAngle = rayAngleStart + x * angleStep;
                rayAngle = NormalizeAngle(rayAngle);

                double distanceToWall = 0;
                bool hitWall = false;
                bool hitDoor = false;
                bool hitKey = false;

                double eyeX = Math.Cos(rayAngle);
                double eyeY = Math.Sin(rayAngle);

                double hitX = 0;
                double hitY = 0;

                // Raycasting loop to find the wall
                while (!hitWall && distanceToWall < maxDepth)
                {
                    distanceToWall += 0.01;

                    int testX = (int)(player.X + eyeX * distanceToWall);
                    int testY = (int)(player.Y + eyeY * distanceToWall);

                    if (testX < 0 || testX >= map.Width || testY < 0 || testY >= map.Height)
                    {
                        hitWall = true;
                        distanceToWall = maxDepth;
                        hitX = player.X + eyeX * distanceToWall;
                        hitY = player.Y + eyeY * distanceToWall;
                        break;
                    }
                    else
                    {
                        CellType cell = map.GetCell(testX, testY);
                        if (cell == CellType.Wall)
                        {
                            hitWall = true;
                            hitX = player.X + eyeX * distanceToWall;
                            hitY = player.Y + eyeY * distanceToWall;
                        }
                        else if (cell == CellType.DoorClosed || cell == CellType.DoorOpen)
                        {
                            hitWall = true;
                            hitDoor = true;
                            hitX = player.X + eyeX * distanceToWall;
                            hitY = player.Y + eyeY * distanceToWall;
                        }
                        else if (cell == CellType.Key)
                        {
                            hitWall = true;
                            hitKey = true;
                            hitX = player.X + eyeX * distanceToWall;
                            hitY = player.Y + eyeY * distanceToWall;
                        }
                    }
                }

                // Apply basic distance correction for perspective
                double correctedDistance = distanceToWall * Math.Cos(rayAngle - player.Angle);
                correctedDistance = Math.Max(0.1, correctedDistance); // Prevent division by zero

                int wallHeight = (int)(ViewHeight / correctedDistance);
                int ceiling = (ViewHeight - wallHeight) / 2;
                int floor = ViewHeight - ceiling;
                int runnerWallStart = floor;
                int runnerWallEnd = Math.Min(ViewHeight, floor + 2); // Runner wall extends 2 rows below the wall base

                // Ceiling character and color
                char ceilingShade = '░';
                ConsoleColor ceilingColor = ConsoleColor.Cyan;

                // Ensure the entire ceiling area is filled uniformly, regardless of distance
                for (int y = 0; y < ceiling && y < ViewHeight; y++)
                {
                    buffer[y, x] = ceilingShade;
                    colorBuffer[y, x] = ceilingColor;
                }

                // Adjust shading based on distance for smoother transition
                char wallShade = correctedDistance < maxDepth / 3 ? '█' : correctedDistance < maxDepth / 2 ? '▓' : '▒';
                char runnerWallShade = correctedDistance < maxDepth / 3 ? '▓' : '▒'; // Darker runner wall for depth

                if (hitDoor) wallShade = '+';
                else if (hitKey) wallShade = 'K';

                // Determine wall orientation
                wallOrientation = DetermineWallOrientation(hitX, hitY, map);

                // Get wall color based on distance and orientation
                ConsoleColor wallColor;
                if (Math.Abs(correctedDistance - previousDistance) < 0.05)
                {
                    wallColor = previousColor; // Use the previous color for very similar distances
                }
                else
                {
                    wallColor = GetWallColor(correctedDistance, maxDepth, wallOrientation);
                }
                previousDistance = correctedDistance;
                previousColor = wallColor;

                // Draw walls and floor, ensuring indices are within bounds
                for (int y = ceiling; y < ViewHeight; y++)
                {
                    if (y < floor)
                    {
                        // Ensure we stay within the array bounds
                        if (y < 0 || y >= ViewHeight) continue;

                        buffer[y, x] = wallShade; // Wall or door

                        // Assign colors based on what was hit
                        if (hitDoor)
                            colorBuffer[y, x] = ConsoleColor.Yellow; // Color for doors
                        else if (hitKey)
                            colorBuffer[y, x] = ConsoleColor.Magenta; // Color for keys
                        else
                            colorBuffer[y, x] = wallColor; // Shaded wall color
                    }
                    else if (y >= runnerWallStart && y < runnerWallEnd)
                    {
                        // Runner wall base shading for smoother transitions
                        buffer[y, x] = runnerWallShade;
                        colorBuffer[y, x] = wallColor; // Use the same shade as the wall above
                    }
                    else if (y >= floor && y < ViewHeight)
                    {
                        // Floor shading with smoother transitions
                        double b = 1.0 - ((double)y - ViewHeight / 2) / (ViewHeight / 2);
                        if (b < 0.25)
                        {
                            buffer[y, x] = '#';
                            colorBuffer[y, x] = ConsoleColor.DarkGreen;
                        }
                        else if (b < 0.5)
                        {
                            buffer[y, x] = 'x';
                            colorBuffer[y, x] = ConsoleColor.Green;
                        }
                        else if (b < 0.75)
                        {
                            buffer[y, x] = '.';
                            colorBuffer[y, x] = ConsoleColor.Gray;
                        }
                        else
                        {
                            buffer[y, x] = ' ';
                            colorBuffer[y, x] = ConsoleColor.Black;
                        }
                    }
                }
            }

            // Render the buffer to the console
            Console.SetCursorPosition(0, 0);
            for (int y = 0; y < ViewHeight; y++)
            {
                for (int x = 0; x < ViewWidth; x++)
                {
                    Console.ForegroundColor = colorBuffer[y, x];
                    Console.Write(buffer[y, x]);
                }
                Console.WriteLine();
            }

            Console.ResetColor();
            // Render the minimap
            RenderMiniMap(map, player);
            return (buffer, colorBuffer);
        }

        private static void RenderMiniMap(Map map, Player player)
        {
            int minimapX = ViewWidth; // Starting x position of minimap
            int minimapY = 0;         // Starting y position of minimap

            // Define the size of the minimap window
            int windowWidth = minimapWidth;
            int windowHeight = minimapHeight;

            // Center the minimap on the player's position
            int startX = (int)player.X - windowWidth / 2;
            int startY = (int)player.Y - windowHeight / 2;

            // Ensure the minimap window doesn't go out of bounds
            if (startX < 0) startX = 0;
            if (startY < 0) startY = 0;
            if (startX + windowWidth > map.Width) startX = map.Width - windowWidth;
            if (startY + windowHeight > map.Height) startY = map.Height - windowHeight;

            for (int y = 0; y < windowHeight; y++)
            {
                Console.SetCursorPosition(minimapX, minimapY + y);
                for (int x = 0; x < windowWidth; x++)
                {
                    int mapX = startX + x;
                    int mapY = startY + y;

                    CellType cell = map.GetCell(mapX, mapY);

                    char displayChar = ' ';
                    ConsoleColor color = ConsoleColor.Black;

                    // Check if the player is at this minimap position
                    if (mapX == (int)player.X && mapY == (int)player.Y)
                    {
                        displayChar = 'P';
                        color = ConsoleColor.Red;
                    }
                    else
                    {
                        switch (cell)
                        {
                            case CellType.Empty:
                                displayChar = '.';
                                color = ConsoleColor.DarkGray;
                                break;
                            case CellType.Wall:
                                displayChar = '#';
                                color = ConsoleColor.Gray;
                                break;
                            case CellType.DoorClosed:
                                displayChar = '+';
                                color = ConsoleColor.DarkYellow;
                                break;
                            case CellType.DoorOpen:
                                displayChar = '/';
                                color = ConsoleColor.Yellow;
                                break;
                            case CellType.Key:
                                displayChar = 'K';
                                color = ConsoleColor.Cyan;
                                break;
                        }
                    }

                    Console.ForegroundColor = color;
                    Console.Write(displayChar);
                }
            }
            Console.ResetColor();
        }

        private static int DetermineWallOrientation(double hitX, double hitY, Map map)
        {
            double fracX = hitX - Math.Floor(hitX);
            double fracY = hitY - Math.Floor(hitY);
            double threshold = 0.1;

            if (fracX < threshold || fracX > 1 - threshold)
            {
                return 0; // Vertical wall (North-South)
            }
            else if (fracY < threshold || fracY > 1 - threshold)
            {
                return 1; // Horizontal wall (East-West)
            }
            else
            {
                return (fracX < fracY) ? 0 : 1;
            }
        }

        private static ConsoleColor GetWallColor(double distance, double maxDepth, int wallOrientation)
        {
            double ratio = distance / maxDepth;

            // Base color selection based on distance
            ConsoleColor baseColor = ratio switch
            {
                < 0.2 => ConsoleColor.White,
                < 0.4 => ConsoleColor.Gray,
                < 0.6 => ConsoleColor.DarkGray,
                < 0.8 => ConsoleColor.DarkBlue,
                _ => ConsoleColor.Black
            };

            // Darken the color for side walls (horizontal walls)
            if (wallOrientation == 1)
            {
                baseColor = DarkenColor(baseColor);
            }

            return baseColor;
        }

        private static ConsoleColor DarkenColor(ConsoleColor color)
        {
            return color switch
            {
                ConsoleColor.White => ConsoleColor.Gray,
                ConsoleColor.Gray => ConsoleColor.DarkGray,
                ConsoleColor.DarkGray => ConsoleColor.DarkBlue,
                ConsoleColor.DarkBlue => ConsoleColor.Black,
                _ => ConsoleColor.Black
            };
        }

        private static void ClearBuffer()
        {
            for (int y = 0; y < ViewHeight; y++)
            {
                for (int x = 0; x < ViewWidth; x++)
                {
                    buffer[y, x] = ' ';
                    colorBuffer[y, x] = ConsoleColor.Black;
                }
            }
        }

        private static double NormalizeAngle(double angle)
        {
            angle = angle % (2 * Math.PI);
            return angle < 0 ? angle + 2 * Math.PI : angle;
        }
    }
}