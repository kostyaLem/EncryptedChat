using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Encrypted.RSADemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var rsa = new RSA();
            int p = 101, q = 103;

            string text = "Привет";

            var data = rsa.Encrypt(p, q, text);
            var result = rsa.Decrypt(10199, 10403, data);

            Console.ReadKey();
        }
    }
}
