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

        /// <summary>
        /// Stone effectiveness for damage calculations.
        /// </summary>
        public static readonly float[,] Effectiveness = new float[,]
        {
            { 1.0f, 1.0f, 1.0f, 1.0f },
            { 0.75f, 1.0f, 1.25f, 1.0f },
            { 1.0f, 0.75f, 1.0f, 1.25f },
            { 1.25f, 1.0f, 0.75f, 1.0f },
            { 1.0f, 1.25f, 1.0f, 0.75f }
        };

        /// <summary>
        /// Stone effectiveness for modifying unit movement.
        /// Order: grenadier, ranger, warrior, cleric, enemy grenadier,
        ///        enemy ranger, enemy warrior, enemy cleric
        /// </summary>
        public static readonly float[,] Movement = new float[,]
        {
            {0.0f, 0.0f, 0.0f, 0.0f},
            {0.0f, 0.0f, 0.0f, 0.0f},
            {0.3f, 0.2f, 0.3f, 0.0f},
            {0.0f, 0.0f, 0.0f, 0.0f},
            {0.3f, 0.3f, 0.0f, 0.2f},
            {0.3f, 0.2f, 0.3f, 0.0f},
            {0.0f, 0.0f, 0.0f, 0.0f},
            {0.0f, 0.0f, 0.0f, 0.0f}
        };

        /// <summary>
        /// Stone effectiveness for base damage boosts.
        /// </summary>
        public static readonly float[,] Damage = new float[,]
        {
            {0.0f, 0.0f, 0.0f, 0.0f},
            {0.2f, 0.0f, 0.3f, 0.3f},
            {0.0f, 0.2f, 0.3f, 0.3f},
            {0.0f, 0.0f, 0.0f, 0.0f},
            {0.0f, 0.0f, 0.0f, 0.0f},
            {0.0f, 0.2f, 0.3f, 0.3f},
            {0.2f, 0.0f, 0.3f, 0.3f},
            {0.0f, 0.0f, 0.0f, 0.0f}
        };

        /// <summary>
        /// Stone effectiveness for defense boosts.
        /// </summary>
        public static readonly float[,] Defense = new float[,]
        {
            {0.3f, 0.3f, 0.2f, 0.0f},
            {0.0f, 0.0f, 0.0f, 0.0f},
            {0.3f, 0.2f, 0.0f, 0.3f},
            {0.0f, 0.0f, 0.0f, 0.0f},
            {0.0f, 0.0f, 0.0f, 0.0f},
            {0.3f, 0.2f, 0.0f, 0.3f},
            {0.0f, 0.0f, 0.0f, 0.0f},
            {0.0f, 0.0f, 0.0f, 0.0f}
        };

        /// <summary>
        /// Stone effectiveness for attack range boosts.
        /// </summary>
        public static readonly float[,] Range = new float[,]
        {
            {0.3f, 0.0f, 0.2f, 0.3f},
            {0.2f, 0.3f, 0.0f, 0.3f},
            {0.0f, 0.0f, 0.0f, 0.0f},
            {0.0f, 0.0f, 0.0f, 0.0f},
            {0.0f, 0.0f, 0.0f, 0.0f},
            {0.0f, 0.0f, 0.0f, 0.0f},
            {0.2f, 0.3f, 0.0f, 0.3f},
            {0.0f, 0.0f, 0.0f, 0.0f}
        };

        public static readonly string[] UIText = new string[]
        {
            "No Stone",
            "Air Stone",
            "Earth Stone",
            "Fire Stone",
            "Water Stone",
        };
    }
}
