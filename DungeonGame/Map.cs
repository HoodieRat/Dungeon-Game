using System;
using System.Collections.Generic;
using System.IO;
using System;
using System.Collections.Generic;
using Microsoft.VisualBasic;
using System.Drawing;
using System.Text;

namespace DungeonGame
{
    // Map Class: Generates and manages the dungeon map
    class Map
    {
        public int Width { get; }
        public int Height { get; }
        private CellType[,] cells;
        private Random rand;
        public int StartX { get; private set; }
        public int StartY { get; private set; }

        public Map(int seed)
        {
            Width = 50;
            Height = 30;
            cells = new CellType[Height, Width];
            rand = new Random(seed);

            GenerateMap();
        }

        private void GenerateMap()
        {
            // Initialize all cells as walls
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                    cells[y, x] = CellType.Wall;

            // Create rooms
            int roomCount = rand.Next(5, 10);
            List<Rectangle> rooms = new List<Rectangle>();

            for (int i = 0; i < roomCount; i++)
            {
                int roomWidth = rand.Next(4, 10);
                int roomHeight = rand.Next(4, 8);
                int roomX = rand.Next(1, Width - roomWidth - 1);
                int roomY = rand.Next(1, Height - roomHeight - 1);

                Rectangle room = new Rectangle(roomX, roomY, roomWidth, roomHeight);
                bool overlaps = false;

                foreach (var otherRoom in rooms)
                {
                    if (room.Intersects(otherRoom))
                    {
                        overlaps = true;
                        break;
                    }
                }

                if (!overlaps)
                {
                    rooms.Add(room);
                    CreateRoom(room);
                }
            }

            // Connect rooms with hallways
            for (int i = 1; i < rooms.Count; i++)
            {
                int x1 = rooms[i - 1].CenterX;
                int y1 = rooms[i - 1].CenterY;
                int x2 = rooms[i].CenterX;
                int y2 = rooms[i].CenterY;

                if (rand.Next(2) == 0)
                {
                    CreateHorizontalTunnel(x1, x2, y1);
                    CreateVerticalTunnel(y1, y2, x2);
                }
                else
                {
                    CreateVerticalTunnel(y1, y2, x1);
                    CreateHorizontalTunnel(x1, x2, y2);
                }
            }

            // Place player in the first room
            StartX = rooms[0].CenterX;
            StartY = rooms[0].CenterY;

            // Place doors and keys
            PlaceDoorsAndKeys(rooms);
        }

        private void CreateRoom(Rectangle room)
        {
            for (int y = room.Y1; y <= room.Y2; y++)
                for (int x = room.X1; x <= room.X2; x++)
                    cells[y, x] = CellType.Empty;
        }

        private void CreateHorizontalTunnel(int x1, int x2, int y)
        {
            for (int x = Math.Min(x1, x2); x <= Math.Max(x1, x2); x++)
                SetCell(x, y, CellType.Empty);
        }

        private void CreateVerticalTunnel(int y1, int y2, int x)
        {
            for (int y = Math.Min(y1, y2); y <= Math.Max(y1, y2); y++)
                SetCell(x, y, CellType.Empty);
        }

        private void PlaceDoorsAndKeys(List<Rectangle> rooms)
        {
            foreach (var room in rooms)
            {
                // Place doors at room exits
                List<(int x, int y)> possiblePositions = new List<(int x, int y)>();

                for (int x = room.X1; x <= room.X2; x++)
                {
                    possiblePositions.Add((x, room.Y1 - 1));
                    possiblePositions.Add((x, room.Y2 + 1));
                }

                for (int y = room.Y1; y <= room.Y2; y++)
                {
                    possiblePositions.Add((room.X1 - 1, y));
                    possiblePositions.Add((room.X2 + 1, y));
                }

                foreach (var pos in possiblePositions)
                {
                    if (pos.x > 0 && pos.x < Width && pos.y > 0 && pos.y < Height)
                    {
                        if (cells[pos.y, pos.x] == CellType.Wall && rand.Next(100) < 20)
                        {
                            cells[pos.y, pos.x] = CellType.DoorClosed;
                        }
                    }
                }

                // Place a key in the room
                int keyX = rand.Next(room.X1 + 1, room.X2);
                int keyY = rand.Next(room.Y1 + 1, room.Y2);
                cells[keyY, keyX] = CellType.Key;
            }
        }

        public CellType GetCell(int x, int y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
                return CellType.Wall;
            return cells[y, x];
        }

        public void SetCell(int x, int y, CellType cellType)
        {
            if (x >= 0 && x < Width && y >= 0 && y < Height)
                cells[y, x] = cellType;
        }

        // Method to save the full map to a .txt file
        public void SaveMapToFile(string filename)
        {
            using (StreamWriter writer = new StreamWriter(filename))
            {
                for (int y = 0; y < Height; y++)
                {
                    StringBuilder line = new StringBuilder();
                    for (int x = 0; x < Width; x++)
                    {
                        char c = ' ';
                        switch (cells[y, x])
                        {
                            case CellType.Empty:
                                c = '.';
                                break;
                            case CellType.Wall:
                                c = '#';
                                break;
                            case CellType.DoorClosed:
                                c = '+';
                                break;
                            case CellType.DoorOpen:
                                c = '/';
                                break;
                            case CellType.Key:
                                c = 'K';
                                break;
                        }
                        line.Append(c);
                    }
                    writer.WriteLine(line.ToString());
                }
            }
        }
    }
}