using System;

namespace VGDC_RPG.Map
{
    public struct Int2Float : IComparable<Int2Float>
    {
        public Int2 Value;
        public float Distance;

        public Int2Float(Int2 v, float d)
        {
            Value = v;
            Distance = d;
        }

        public int CompareTo(Int2Float other)
        {
            if (other.Distance > Distance)
                return -1;
            if (other.Distance < Distance)
                return 1;
            return 0;
        }
    }
}
