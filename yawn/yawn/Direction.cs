using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace yawn
{
    class Direction
    {
        public static Direction EAST = new Direction(0);
        public static Direction NORTH = new Direction(90);
        public static Direction WEST = new Direction(180);
        public static Direction SOUTH = new Direction(270);

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
                    name = "East";
                    break;
                case 1:
                    name = "North";
                    break;
                case 2:
                    name = "West";
                    break;
                case 3:
                    name = "South";
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

        public Direction Next()
        {
            return new Direction(degrees + 90);
        }

        public Direction Opposite()
        {
            return new Direction(degrees + 180);
        }

        public Tuple<int, int> Move(Tuple<int, int> Position) {
            switch(ordinal) {
                default:
                case 0:
                    return new Tuple<int, int>(Position.Item1 + 1, Position.Item2);
                case 1:
                    return new Tuple<int, int>(Position.Item1, Position.Item2 - 1);
                case 2:
                    return new Tuple<int, int>(Position.Item1 - 1, Position.Item2);
                case 3:
                    return new Tuple<int, int>(Position.Item1, Position.Item2 + 1);
            }
                
        }

    }
}
