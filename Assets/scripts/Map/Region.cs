using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.scripts.Map
{
    public static class Region
    {
        private static List<ushort> registeredRegions = new List<ushort>();

        static Region()
        {
            RegisterRegion(4);
        }
        
        public static void RegisterRegion(ushort baseID)
        {
            registeredRegions.Add(baseID);
        }

        public static ushort GetTile(ushort[,] m, int x, int y)
        {
            ushort v = m[x, y];
            if (!registeredRegions.Contains(v))
                return v;
            
            var left = x == 0 || m[x - 1, y] == v;
            var right = x == m.GetLength(0) - 1 || m[x + 1, y] == v;
            var down = y == 0 || m[x, y - 1] == v;
            var up = y == m.GetLength(1) - 1 || m[x, y + 1] == v;

            var leftdown = x == 0 || y == 0 || m[x - 1, y - 1] == v;
            var rightdown = x == m.GetLength(0) - 1 || y == 0 || m[x + 1, y - 1] == v;
            var leftup = x == 0 || y == m.GetLength(1) - 1 || m[x - 1, y + 1] == v;
            var rightup = x == m.GetLength(0) - 1 || y == m.GetLength(1) - 1 || m[x + 1, y + 1] == v;

            if (left && right && up && down && leftup && rightdown && !leftdown && !rightup)
                return (ushort)(v + 10);
            if (left && right && up && down && !leftup && !rightdown && leftdown && rightup)
                return (ushort)(v + 11);

            if ((right && down && rightdown) && !(up && rightup) && !(left && leftdown))
                return (ushort)(v + 1);
            if (left && right && down && leftdown && rightdown && (!up || (!leftup && !rightup)))
                return (ushort)(v + 2);
            if ((left && down && leftdown) && !(up && leftup) && !(right && rightdown))
                return (ushort)(v + 3);

            if (right && up && down && rightup && rightdown && (!left || (!leftup && !leftdown)))
                return (ushort)(v + 4);
            if (left && up && right && down && leftup && rightup && leftdown && rightdown)
                return v;
            if (left && up && down && leftup && leftdown && (!right || (!rightup && !rightdown)))
                return (ushort)(v + 5);

            if ((right && up && rightup) && !(down && rightdown) && !(left && leftup))
                return (ushort)(v + 6);
            if (left && right && up && leftup && rightup && (!down || (!leftdown && !rightdown)))
                return (ushort)(v + 7);
            if ((left && up && leftup) && !(down && leftdown) && !(right && rightup))
                return (ushort)(v + 8);

            if (left && right && up && down && leftdown && rightup && rightdown && !leftup)
                return (ushort)(v + 12);
            if (left && right && up && down && leftdown && !rightup && rightdown && leftup)
                return (ushort)(v + 13);
            if (left && right && up && down && !leftdown && rightup && rightdown && leftup)
                return (ushort)(v + 14);
            if (left && right && up && down && leftdown && rightup && !rightdown && leftup)
                return (ushort)(v + 15);

            return (ushort)(v + 9);
        }
    }
}
