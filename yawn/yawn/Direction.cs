using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace yawn
{
    class Direction
    {
        public static Direction EAST = new Direction(90);
        public static Direction SOUTH = new Direction(180);
        public static Direction WEST = new Direction(270);
        public static Direction NORTH = new Direction(0);

        private string name;
        private int ordinal;
        private int degrees;

        public Direction(int Degrees)
        {
            int NormDeg = Degrees % 360;
            degrees = NormDeg;
            ordinal = NormDeg / 90;
            switch (ordinal)
            {
                case 0:
                    name = "North";
                    break;
                case 1:
                    name = "East";
                    break;
                case 2:
                    name = "South";
                    break;
                case 3:
                    name = "West";
                    break;
            }

        }
        public override string ToString()
        {
            return name;
        }

        public int Ordinal()
        {
            return ordinal;
        }

        public int Degrees()
        {
            return degrees;
        }

        public float RotationAngle()
        {
            return ((float)degrees - 90.0f) * (float)Math.PI / 180.0f;
        }

        public Direction Next()
        {
            return new Direction(degrees + 90);
        }

        public Direction Opposite()
        {
            return new Direction(degrees + 180);
        }

        public Vector2 Move(Vector2 Position) {
            switch(ordinal) {
                default:
                case 0:
                    return new Vector2(Position.X, Position.Y - 1);
                case 1:
                    return new Vector2(Position.X + 1, Position.Y);
                case 2:
                    return new Vector2(Position.X, Position.Y + 1);
                case 3:
                    return new Vector2(Position.X - 1, Position.Y);
            }
                
        }

    }
}
