using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Encrypted
{
    public class RSA
    {
        public List<string> Encrypt(long p, long q, string text)
        {
            if (p.IsSimpleNum() && q.IsSimpleNum())
            {
                var n = p * q;
                var fi = (p - 1) * (q - 1);

                long d = GetPrivatePartKey(fi);
                long e = GetPublicPartKey(d, fi);

                return Encode(text, e, n);
            }
            else
                throw new ArgumentException("p or n is't simple numbers");
        }

        public string Decrypt(long d, long n, List<string> data)
        {
            return Decode(data, d, n);
        }

        private List<string> Encode(string text, long e, long n)
        {
            var data = new List<string>();

            BigInteger num;

            foreach (var ch in text)
            {
                int index = ch;

                num = new BigInteger(index);
                num = BigInteger.Pow(num, (int)e);

                BigInteger mod = new BigInteger((int)n);
                num = num % mod;

                data.Add(num.ToString());
            }

            return data;
        }

        private string Decode(List<string> data, long d, long n)
        {
            var strBuilder = new StringBuilder();

            BigInteger num;

            foreach (var item in data)
            {
                num = new BigInteger(Convert.ToInt64(item));
                num = BigInteger.Pow(num, (int)d);

                var mod = new BigInteger((int)n);
                num = num % mod;

                int index = Convert.ToInt32(num.ToString());

                strBuilder.Append((char)index);
            }

            return strBuilder.ToString();
        }

        private long GetPrivatePartKey(long fi)
        {
            long d = fi - 1;

            for (long i = 2; i <= fi; i++)
            {
                if ((fi % i == 0) && (d % i == 0))
                {
                    d--;
                    i = 1;
                }
            }
            return d;
        }

        private long GetPublicPartKey(long d, long fi)
        {
            long e = 10;

            while (true)
            {
                if ((e * d) % fi == 1)
                    break;
                else
                    e++;
            }

            return e;
        }
    }
}
