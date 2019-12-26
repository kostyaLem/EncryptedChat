using System;

namespace Encrypted.RSADemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var client1 = new RSA();
            var client2 = new RSA();

            string text = "Привет всем. Суки!";

            var data = client2.Encrypt(text, client1.e, client1.n);

            var result = client1.Decrypt(data);

            Console.ReadKey();
        }
    }
}
