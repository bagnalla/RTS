using System;
using Microsoft.Xna.Framework;

namespace rts
{
    public class Direction
    {
        private static readonly
            Direction north = new Direction(270),
            south = new Direction(90),
            west = new Direction(180),
            east = new Direction(0),
            northEast = new Direction(315),
            northWest = new Direction(225),
            southEast = new Direction(45),
            southWest = new Direction(135);

        private static readonly Direction[] cardinals = new Direction[] { north, south, west, east };
        private static readonly Direction[] intercardinals = new Direction[] { northEast, northWest, southEast, southWest };
        private static readonly Direction[] directions = new Direction[] { north, south, west, east, northEast, northWest, southEast, southWest };

        private Direction(float angle)
        {
            Angle = MathHelper.WrapAngle(MathHelper.ToRadians(angle));
            X = (float)Math.Cos(Angle);
            Y = (float)Math.Sin(Angle);
        }

        // call at game initialization to load class early
        public static void Init() { }

        public float Angle { get; private set; }
        public float X { get; private set; }
        public float Y { get; private set; }

        public static Direction North
        {
            get
            {
                return north;
            }
        }
        public static Direction South
        {
            get
            {
                return south;
            }
        }
        public static Direction West
        {
            get
            {
                return west;
            }
        }
        public static Direction East
        {
            get
            {
                return east;
            }
        }
        public static Direction NorthEast
        {
            get
            {
                return northEast;
            }
        }
        public static Direction SouthEast
        {
            get
            {
                return southEast;
            }
        }
        public static Direction NorthWest
        {
            get
            {
                return northWest;
            }
        }
        public static Direction SouthWest
        {
            get
            {
                return southWest;
            }
        }
        public static Direction[] Cardinals
        {
            get
            {
                return cardinals;
            }
        }
        public static Direction[] Intercardinals
        {
            get
            {
                return intercardinals;
            }
        }
        public static Direction[] Directions
        {
            get
            {
                return directions;
            }
        }

        public override bool Equals(object o)
        {
            if (o == null)
                return false;

            var other = o as Direction;

            return (other != null && Angle == other.Angle);
        }
        public override int GetHashCode()
        {
            return (int)(Angle * 100);
        }

        public static bool operator ==(Direction d1, Direction d2)
        {
            return d1.Equals(d2);
        }
        public static bool operator !=(Direction d1, Direction d2)
        {
            return !d1.Equals(d2);
        }
    }
}
