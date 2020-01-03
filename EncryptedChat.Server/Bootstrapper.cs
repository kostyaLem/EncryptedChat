using System;
using System.Net;
using System.Net.Sockets;

namespace EncryptedChat.Server
{
    internal class Bootstrapper
    {
        static void Main(string[] args)
        {
            if (args.Length == 2 &&
                IPAddress.TryParse(args[0], out IPAddress host) &&
                int.TryParse(args[1], out int port))
            {
                new Server(host, port).Start();
            }
            else
            {
                new Server(GetLocalIPAddress(), 5050).Start();
            }
        }

        public static IPAddress GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    return ip;

            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
    }
}
