using System;
using Microsoft.DirectX;

namespace HMapEdit {
    public static class Utils {
        private static readonly Random RANDOM = new Random();

        public static double GetDistance(Vector3 v1, Vector3 v2) {
            float relX = v2.X - v1.X;
            float relY = v2.Y - v1.Y;
            float relZ = v2.Z - v1.Z;

            return Math.Sqrt(relX*relX + relY*relY + relZ*relZ);
        }

        public static double Get2DDistance(Vector3 v1, Vector3 v2) {
            float relX = v2.X - v1.X;
            float relY = v2.Y - v1.Y;

            return Math.Sqrt(relX*relX + relY*relY);
        }

        public static double RandomDouble() {
            return RANDOM.NextDouble();
        }

        public static int RandomInt(int min, int max) {
            return RANDOM.Next(min, max);
        }

        public static float LimitTC(float val) {
            return Math.Min(Math.Max(val, 0f), 1f);
        }
    }
}