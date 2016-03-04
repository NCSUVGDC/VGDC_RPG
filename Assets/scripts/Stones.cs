using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGDC_RPG
{
    public static class Stones
    {
        public const int COUNT = 4;

        public const int NO_STONE = 0;
        public const int AIR_STONE = 1;
        public const int EARTH_STONE = 2;
        public const int FIRE_STONE = 3;
        public const int WATER_STONE = 4;

        public static readonly string[] UIText = new string[]
        {
            "No Stone",
            "Air Stone",
            "Earth Stone",
            "Fire Stone",
            "Water Stone"
        };
    }
}
