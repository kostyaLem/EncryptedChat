using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Encrypted
{
    public class RSA
    {
        private bool _isReady;
        private long local_D;

        public long p;
        public long q;
        public long n;
        public long e;

        public RSA()
        {
            Initialize();
        }

        public void Initialize()
        {
            p = PrimeNumberGenerator.Generate();
            q = PrimeNumberGenerator.Generate();

            n = p * q;

            var fi = (p - 1) * (q - 1);

            e = GetPublicPartKey(fi);
            local_D = GetPrivatePartKey(fi, e);

            _isReady = true;
        }

        public string[] Encrypt(string text, long publicE, long publicN)
        {
            if (!_isReady)
                throw new ArgumentException("Method Initialize not called");

            return Encode(text, publicE, publicN);
        }

        public string Decrypt(string[] data)
        {
            if (!_isReady)
                throw new ArgumentException("Method Initialize not called");

            return Decode(data, local_D, n);
        }

        private string[] Encode(string text, long e, long n)
        {
            var data = new List<string>();

            BigInteger num;

            foreach (var ch in text)
            {
                int index = ch;

                num = BigInteger.ModPow(index, e, n);

                data.Add(num.ToString());
            }

            return data.ToArray();
        }

        private string Decode(string[] data, long d, long n)
        {
                var strBuilder = new StringBuilder();

                BigInteger num;

                foreach (var item in data)
                {
                    var val = new BigInteger(Convert.ToInt64(item));
                    num = BigInteger.ModPow(val, d, n);

                    strBuilder.Append((char)num);
                }

                return strBuilder.ToString();
        }

        private long GetPrivatePartKey(long fi, long e)
        {
            long d = e + 1;

            while (true)
            {
                if ((d * e) % fi == 1)
                    break;
                d++;
            }

            return d;
        }

        private long GetPublicPartKey(long fi)
        {
            long e = fi - 1;

            while (true)
            {
                if (PrimeNumberGenerator.IsPrime(e) &&
                    e < fi &&
                    BigInteger.GreatestCommonDivisor(new BigInteger(e), new BigInteger(fi)) == BigInteger.One)
                    break;
                e--;
            }

            return e;
        }
    }
}
