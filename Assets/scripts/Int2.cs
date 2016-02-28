using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGDC_RPG
{
    public struct Int2
    {
        public int X, Y;

        public Int2(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static bool operator ==(Int2 a, Int2 b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        public static bool operator !=(Int2 a, Int2 b)
        {
            return a.X != b.X || a.Y != b.Y;
        }

        public override bool Equals(object obj)
        {
            if (obj is Int2)
                return this == (Int2)obj;
            return false;
        }

        public override int GetHashCode()
        {
            int prime = 31;
            int result = 1;
            result = prime * result + X;
            result = prime * result + Y;
            return result;
        }
    }
}
