using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Encrypted
{
    public static class NumericExtensions
    {
        public static bool IsSimpleNum(this long num)
        {
            if (num < 2)
                return false;

            if (num == 2)
                return true;

            // TODO: Fix it
            for (long i = 2; i < num; i++)
                if (num % i == 0)
                    return false;

            return true;
        }
    }
}
