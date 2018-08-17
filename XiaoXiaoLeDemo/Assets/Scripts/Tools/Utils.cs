using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Berry.Utils
{
    public enum Side
    {
        Null, Top, Bottom, Right, Left,
        TopRight, TopLeft,
        BottomRight, BottomLeft
    }
    public static class Utils
    {
        public static readonly Side[] allSides = {
                                                     Side.Top,          //1
                                                     Side.TopRight,     //5
                                                     Side.Right,        //3
                                                     Side.BottomRight,  //7
                                                     Side.Bottom,       //2
                                                     Side.BottomLeft,   //8
                                                     Side.Left,         //4
                                                     Side.TopLeft       //6
                                                 };

        public static readonly Side[] straightSides = { Side.Top, Side.Bottom, Side.Right, Side.Left };
        public static readonly Side[] slantedSides = { Side.TopLeft, Side.TopRight, Side.BottomRight, Side.BottomLeft };

        public static T GetRandom<T>(this ICollection<T> collection)
        {
            if (collection == null)
                return default(T);
            int t = UnityEngine.Random.Range(0, collection.Count);
            foreach (T element in collection)
            {
                if (t == 0)
                    return element;
                t--;
            }
            return default(T);
        }
        public static Int2 SideOffset(Side s)
        {
            switch (s)
            {
                case Side.Right: return new Int2(1, 0);
                case Side.TopRight: return new Int2(1, 1);
                case Side.Top: return new Int2(0, 1);
                case Side.TopLeft: return new Int2(-1, 1);
                case Side.Left: return new Int2(-1, 0);
                case Side.BottomLeft: return new Int2(-1, -1);
                case Side.Bottom: return new Int2(0, -1);
                case Side.BottomRight: return new Int2(1, -1);
                default: return new Int2(0, 0);
            }
        }
        public static float SideToAngle(Side s)
        {
            switch (s)
            {
                case Side.Right: return 0;
                case Side.TopRight: return 45;
                case Side.Top: return 90;
                case Side.TopLeft: return 135;
                case Side.Left: return 180;
                case Side.BottomLeft: return 225;
                case Side.Bottom: return 270;
                case Side.BottomRight: return 315;
                default: return 0;
            }
        }
    }
    [System.Serializable]
    public class Int2
    {
        public static readonly Int2 right = new Int2(1, 0);
        public static readonly Int2 up = new Int2(0, 1);
        public static readonly Int2 left = new Int2(-1, 0);
        public static readonly Int2 down = new Int2(0, -1);
        public static readonly Int2 Null = new Int2(int.MinValue, int.MinValue);

        public int x;
        public int y;

        public Int2()
        {
            x = 0;
            y = 0;
        }
        public Int2(int _x, int _y)
        {
            x = _x;
            y = _y;
        }

        public static bool operator == (Int2 a, Int2 b)
        {
            if ((object)a == null)
                return (object)b == null;
            return a.Equals(b);
        }
        public static bool operator != (Int2 a, Int2 b)
        {
            if ((object)a == null)
                return (object)b != null;
            return !a.Equals(b);
        }
        public static Int2 operator *(Int2 a, int b)
        {
            return new Int2(a.x * b, a.y * b);
        }

        public static Int2 operator *(int b, Int2 a)
        {
            return a * b;
        }
        public static Int2 operator +(Int2 a, Int2 b)
        {
            return new Int2(a.x + b.x, a.y + b.y);
        }

        public static Int2 operator -(Int2 a, Int2 b)
        {
            return new Int2(a.x - b.x, a.y - b.y);
        }
        public static Int2 operator +(Int2 a, Side side)
        {
            return a + Utils.SideOffset(side);
        }
        public static Int2 operator -(Int2 a, Side side)
        {
            return a - Utils.SideOffset(side);
        }
        public bool IsItHit(int min_x, int min_y, int max_x, int max_y)
        {
            return x >= min_x && x <= max_x && y >= min_y && y <= max_y;
        }
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Int2))
                return false;
            Int2 b = (Int2)obj;
            return x == b.x && y == b.y;
        }
        public override int GetHashCode()
        {
            return x.GetHashCode() + y.GetHashCode();
        }
        public override string ToString()
        {
            return "(" + x + ", " + y + ")";
        }
        public Int2 GetClone()
        {
            return (Int2)MemberwiseClone();
        }
    }
}
