namespace VGDC_RPG
{
    /// <summary>
    /// 2 component vector of integer values.
    /// </summary>
    public struct Int2
    {
        /// <summary>
        /// The X component of the vector.
        /// </summary>
        public int X;
        /// <summary>
        /// The Y component of the vector.
        /// </summary>
        public int Y;

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
            // May be flawed...
            int prime = 31;
            int result = 1;
            result = prime * result + X;
            result = prime * result + Y;
            return result;
        }

        public override string ToString()
        {
            return "{" + X + ", " + Y + "}";
        }
    }
}
