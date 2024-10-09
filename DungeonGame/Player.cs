using DungeonGame;
using System;

namespace DungeonGame
{
    class Player
    {
        public double X { get; private set; }
        public double Y { get; private set; }
        public double Angle { get; private set; } = -Math.PI / 2; // Facing North
        public int Keys { get; private set; } = 0;
        private double moveSpeed = 1;
        private double rotationSpeed = 0.1;

        public Player(int startX, int startY)
        {
            X = startX + 0.5;
            Y = startY + 0.5;
        }

        public Direction FacingDirection
        {
            get
            {
                if (Angle >= -Math.PI / 4 && Angle < Math.PI / 4)
                    return Direction.East;
                else if (Angle >= Math.PI / 4 && Angle < 3 * Math.PI / 4)
                    return Direction.South;
                else if (Angle >= -3 * Math.PI / 4 && Angle < -Math.PI / 4)
                    return Direction.North;
                else
                    return Direction.West;
            }
        }

        public void MoveForward(Map map)
        {
            double newX = X + Math.Cos(Angle) * moveSpeed;
            double newY = Y + Math.Sin(Angle) * moveSpeed;
            AttemptMove(newX, newY, map);
        }

        public void MoveBackward(Map map)
        {
            double newX = X - Math.Cos(Angle) * moveSpeed;
            double newY = Y - Math.Sin(Angle) * moveSpeed;
            AttemptMove(newX, newY, map);
        }

        public void StrafeRight(Map map)
        {
            double newX = X - Math.Sin(Angle) * moveSpeed; // Adjusted to match full block movement
            double newY = Y + Math.Cos(Angle) * moveSpeed; // Adjusted to match full block movement
            AttemptMove(newX, newY, map);
        }

        public void StrafeLeft(Map map)
        {
            double newX = X + Math.Sin(Angle) * moveSpeed; // Adjusted to match full block movement
            double newY = Y - Math.Cos(Angle) * moveSpeed; // Adjusted to match full block movement
            AttemptMove(newX, newY, map);
        }

        public void RotateLeft()
        {
            Angle -= Math.PI / 2; // Rotate by 90 degrees left
            Angle = NormalizeAngle(Angle);
        }

        public void RotateRight()
        {
            Angle += Math.PI / 2; // Rotate by 90 degrees right
            Angle = NormalizeAngle(Angle);
        }

        public bool IsSpaceRestricted(Map map)
        {
            int leftX = (int)(X + Math.Cos(Angle + Math.PI / 2));
            int leftY = (int)(Y + Math.Sin(Angle + Math.PI / 2));
            int rightX = (int)(X + Math.Cos(Angle - Math.PI / 2));
            int rightY = (int)(Y + Math.Sin(Angle - Math.PI / 2));

            // Restricted if both sides are walls (space limited to a single character)
            return (map.GetCell(leftX, leftY) == CellType.Wall && map.GetCell(rightX, rightY) == CellType.Wall);
        }

        public void Interact(Map map)
        {
            int targetX = (int)(X + Math.Cos(Angle));
            int targetY = (int)(Y + Math.Sin(Angle));
            CellType cell = map.GetCell(targetX, targetY);

            if (cell == CellType.DoorClosed && Keys > 0)
            {
                Keys--;
                map.SetCell(targetX, targetY, CellType.DoorOpen);
                Console.SetCursorPosition(0, 25);
                Console.WriteLine("You unlocked the door.");
            }
            else if (cell == CellType.DoorClosed)
            {
                Console.SetCursorPosition(0, 25);
                Console.WriteLine("The door is locked. You need a key.");
            }
        }

        private static double NormalizeAngle(double angle)
        {
            angle = angle % (2 * Math.PI);
            if (angle < 0)
                angle += 2 * Math.PI;
            return angle;
        }

        private void AttemptMove(double newX, double newY, Map map)
        {
            CellType cell = map.GetCell((int)newX, (int)newY);
            if (cell == CellType.Empty || cell == CellType.DoorOpen)
            {
                X = newX;
                Y = newY;
            }
            else if (cell == CellType.Key)
            {
                Keys++;
                map.SetCell((int)newX, (int)newY, CellType.Empty);
                X = newX;
                Y = newY;
                Console.SetCursorPosition(0, 25);
                Console.WriteLine("You picked up a key!");
            }
            else if (cell == CellType.DoorClosed)
            {
                Console.SetCursorPosition(0, 25);
                Console.WriteLine("The door is locked. Press Space to use a key.");
            }
            else
            {
                // Wall or invalid cell
            }
        }
    }
}