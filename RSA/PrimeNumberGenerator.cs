using System;
using System.Collections.Generic;

namespace Encrypted
{
    internal class PrimeNumberGenerator
    {
        private static Random _random = new Random();

        public static long Generate()
        {
            var values = new List<int>();

            int left = 500, right = 1500;
            while (left++ < right)
            {
                if (IsPrime(left))
                    values.Add(left);
            }

            return values[_random.Next(0, values.Count)];
        }

        public static bool IsPrime(long n)
        {
            if (n == 1)
                return false;

            for (int d = 2; d * d <= n; d++)
                if (n % d == 0)
                    return false;

            return true;
        }
    }
}
