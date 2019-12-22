using EncryptedChat.Server.ClientModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace EncryptedChat.Server
{
    class Server
    {
        static object _consoleObj = new object();

        static TcpListener _listener;
        static List<ServerClient> _clients;

        static Server()
        {
            Console.WriteLine("Введите порт для прослушивания: ");
            var port = "5050";

            _listener = new TcpListener(GetLocalIPAddress(), int.Parse(port));
            _clients = new List<ServerClient>();
        }

        public static IPAddress GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip;
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        static void Main(string[] args)
        {
            try
            {
                _listener.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Не удалось запустить сервер: {e.Message}");
                Console.ReadKey();
                Environment.Exit(0);
            }

            try
            {
                while (true)
                {
                    var client = _listener.AcceptTcpClient();

                    Task.Factory.StartNew(() =>
                    {
                        var stream = client.GetStream();

                        while (client.Connected)
                        {
                            var conClient = (new BinaryFormatter().Deserialize(stream)) as ConnectedClient;

                            if (_clients.Exists(c => c.Login == conClient.Login))
                            {

                            }
                            else
                            {
                                _clients.Add(new ServerClient(client, conClient.ID, conClient.Login));
                                WriteSignalConnection(conClient);
                                SendToAllClients(Extensions.SerializeObject(conClient));
                            }

                            break;
                        }

                        while (client.Connected)
                        {
                            try
                            {
                                var encObject = (new BinaryFormatter().Deserialize(stream) as EncryptedObject);

                                WriteTextToConsole(encObject);
                                SendToAllClients(Extensions.SerializeObject(encObject));
                            }
                            catch (Exception e)
                            {
                                WriteExceptionToConsole(e);
                            }
                        }
                    }, TaskCreationOptions.LongRunning);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"FATAL ERROR: {e.Message}");
            }
        }

        static async void SendToAllClients(byte[] message)
        {
            await Task.Factory.StartNew(() =>
            {
                lock (_clients)
                {
                    foreach (var client in _clients.ToArray())
                    {
                        try
                        {
                            if (client.Client.Connected)
                                client.Client.GetStream().Write(message, 0, message.Length);
                            else
                            {
                                _clients.Remove(client);
                            }
                        }
                        catch (Exception e)
                        {
                            WriteExceptionToConsole(e);
                        }
                    }

                    var deletedClients = _clients.Where(client => !client.Client.Connected).ToList();
                    deletedClients.Select(c => _clients.Remove(c));
                }
            });
        }

        private static void WriteExceptionToConsole(Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(e.Message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static void WriteTextToConsole(EncryptedObject eObj)
        {
            lock (_consoleObj)
            {
                Console.WriteLine($"{eObj.Client.Login} [{eObj.Message.SendTime.ToString()}]: {eObj.Message.Content}");
            }
        }

        private static void WriteSignalConnection(ConnectedClient connectedClient)
        {
            lock (_consoleObj)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"New connection: ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"{connectedClient.Login} - {connectedClient.Source}");
            }
        }
    }
}