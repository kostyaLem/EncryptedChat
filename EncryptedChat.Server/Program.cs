using EncryptedChat.Server.ClientModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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

            _listener = new TcpListener(IPAddress.Any, int.Parse(port));
            _clients = new List<ServerClient>();
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
                        ConnectedClient conClient;

                        var sr = client.GetStream();

                        var clientBytes = new List<byte>();

                        while (client.Connected)
                        {
                            do
                            {
                                var bytes = new byte[256];
                                sr.Read(bytes, 0, bytes.Length);
                                clientBytes.AddRange(bytes);
                            } while (sr.DataAvailable && client.Connected);

                            if (clientBytes.Count != 0 && client.Connected)
                            {
                                conClient = Extensions.DeserializeObject<ConnectedClient>(clientBytes.ToArray());

                                if (_clients.Exists(c => c.ID == conClient.ID))
                                {

                                }
                                else
                                {
                                    _clients.Add(new ServerClient(client, conClient.ID, conClient.Login));
                                    WriteSignalConnection(conClient);
                                    SendToAllClients(clientBytes.ToArray());
                                }

                                break;
                            }
                        }

                        while (client.Connected)
                        {
                            try
                            {
                                var messageBytes = new List<byte>();
                                do
                                {
                                    var bytes = new byte[256];
                                    sr.Read(bytes, 0, bytes.Length);
                                    messageBytes.AddRange(bytes);
                                } while (sr.DataAvailable);
                                if (messageBytes.Count != 0)
                                {
                                    SendToAllClients(messageBytes.ToArray());
                                }
                            }
                            catch { }
                        }
                    });
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
                    foreach (var client in _clients)
                    {
                        try
                        {
                            if (client.Client.Connected)
                            {
                                var sw = client.Client.GetStream();
                                using (var sr = new MemoryStream(message))
                                {
                                    while (sr.Position != sr.Length)
                                    {
                                        var bytes = new byte[256];
                                        sr.Read(bytes, 0, bytes.Length);
                                        sw.Write(bytes, 0, bytes.Length);
                                    }

                                    sw.Flush();
                                }
                            }
                            else continue;
                        }
                        catch (Exception e)
                        {

                        }
                    }

                    var deletedClients = _clients.Where(client => !client.Client.Connected).ToList();
                    deletedClients.Select(c => _clients.Remove(c));
                }
            });
        }

        private static void WriteSignalConnection(ConnectedClient connectedClient)
        {
            lock (_consoleObj)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"New connection:\t");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($"{connectedClient.Login} - {connectedClient.Source}");
            }
        }
    }
}