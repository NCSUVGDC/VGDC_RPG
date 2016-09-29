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

        public static readonly float[,] Effectiveness = new float[,]
        {
            { 1.0f, 1.0f, 1.0f, 1.0f },
            { 0.75f, 1.0f, 1.25f, 1.0f },
            { 1.0f, 0.75f, 1.0f, 1.25f },
            { 1.25f, 1.0f, 0.75f, 1.0f },
            { 1.0f, 1.25f, 1.0f, 0.75f }
        };

        public static readonly string[] UIText = new string[]
        {
            "No Stone",
            "Water Stone",
            "Earth Stone",
            "Fire Stone",
            "Air Stone",
        };
    }
}
