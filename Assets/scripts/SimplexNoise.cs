using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace VGDC_RPG
{
    public class SimplexNoise
    {
        private int[] A = new int[3];
        private float s, u, v, w;
        private int i, j, k;
        private float inv3 = 0.333333333f;
        private float inv6 = 0.166666667f;
        private int[] T;

        public SimplexNoise()
        {
            System.Random rnd = new System.Random();
            T = new int[8];
            for (int q = 0; q < 8; q++)
                T[q] = rnd.Next();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="seed">Must be length 8</param>
        public SimplexNoise(int[] seed)
        {
            if (seed.Length != 8)
                throw new ArgumentException("Seed must be length 8.", "seed");
            T = seed;
        }

        public string GetSeedString()
        {
            StringBuilder seed = new StringBuilder();

            for (int q = 0; q < 8; q++)
            {
                seed.AppendFormat("X8", T[q]);
                if (q < 7)
                    seed.Append(" ");
            }

            return seed.ToString();
        }
        
        public float Noise(float x, float y, float z)
        {
            s = (x + y + z) * inv3;
            i = FastFloor(x + s);
            j = FastFloor(y + s);
            k = FastFloor(z + s);

            s = (i + j + k) * inv6;
            u = x - i + s;
            v = y - j + s;
            w = z - k + s;

            A[0] = 0; A[1] = 0; A[2] = 0;

            int hi = u >= w ? u >= v ? 0 : 1 : v >= w ? 1 : 2;
            int lo = u < w ? u < v ? 0 : 1 : v < w ? 1 : 2;

            return (Kay(hi) + Kay(3 - hi - lo) + Kay(lo) + Kay(0));
        }

        private float Kay(int a)
        {
            s = (A[0] + A[1] + A[2]) * inv6;
            float x = u - A[0] + s;
            float y = v - A[1] + s;
            float z = w - A[2] + s;
            float t = 0.6f - x * x - y * y - z * z;
            int h = Shuffle(i + A[0], j + A[1], k + A[2]);
            A[a]++;
            if (t < 0) return 0;
            int b5 = h >> 5 & 1;
            int b4 = h >> 4 & 1;
            int b3 = h >> 3 & 1;
            int b2 = h >> 2 & 1;
            int b1 = h & 3;

            float p = b1 == 1 ? x : b1 == 2 ? y : z;
            float q = b1 == 1 ? y : b1 == 2 ? z : x;
            float r = b1 == 1 ? z : b1 == 2 ? x : y;

            p = b5 == b3 ? -p : p;
            q = b5 == b4 ? -q : q;
            r = b5 != (b4 ^ b3) ? -r : r;
            t *= t;
            return 8 * t * t * (p + (b1 == 0 ? q + r : b2 == 0 ? q : r));
        }

        private int Shuffle(int i, int j, int k)
        {
            return b(i, j, k, 0) + b(j, k, i, 1) + b(k, i, j, 2) + b(i, j, k, 3) + b(j, k, i, 4) + b(k, i, j, 5) + b(i, j, k, 6) + b(j, k, i, 7);
        }

        private int b(int i, int j, int k, int B)
        {
            return T[b(i, B) << 2 | b(j, B) << 1 | b(k, B)];
        }

        private int b(int N, int B)
        {
            return N >> B & 1;
        }

        private int FastFloor(float n)
        {
            return n > 0 ? (int)n : (int)n - 1;
        }
    }
}
