using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGDC_RPG.Map
{
    public struct Int2Float
    {
        public Int2 Value;
        public float Distance;

        public Int2Float(Int2 v, float d)
        {
            Value = v;
            Distance = d;
        }
    }
}
