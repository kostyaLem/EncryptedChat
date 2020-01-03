using EncryptedChat.Server.ClientModel;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;

namespace EncryptedChat.Server
{
    internal class Server
    {
        static TcpListener _listener;
        static List<ServerClient> _clients;

        internal Server(IPAddress host, int port)
        {            
            _listener = new TcpListener(host, 5050);
            _clients = new List<ServerClient>();

            Console.WriteLine($"Создание сервера на: {host}:{port}");
        }
         
        public void Start()
        {
            try
            {
                _listener.Start();
                Console.WriteLine("Сервер запущен");
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
                        var bf = new BinaryFormatter();
                        var serverClient = new ServerClient();

                        var conClient = bf.Deserialize(stream) as ConnectedClient;

                        if (_clients.Exists(c => c.Login == conClient.Login))
                        {
                            // Write to console about it
                        }
                        else
                        {
                            serverClient = new ServerClient(client, conClient.ID, conClient.Login);
                            _clients.Add(serverClient);
                            WriteSignalAboutConnection(conClient);
                            SendToAllClients(conClient);
                        }

                        while (client.Client.Connected)
                        {
                            try
                            {
                                var encObject = bf.Deserialize(stream) as EncryptedObject;

                                WriteTextToConsole(encObject);
                                SendToAllClients(encObject);
                            }
                            catch (Exception e)
                            {
                                if (client.Client.Poll(0, SelectMode.SelectRead))
                                {
                                    var buff = new byte[1];
                                    if (client.Client.Connected == false)
                                    {
                                        client.Client.Disconnect(true);
                                    }
                                }

                                WriteExceptionToConsole(e);
                                _clients.RemoveAll(x => !x.Client.Connected);
                                Thread.CurrentThread.Abort();
                            }
                        }

                        _clients.Remove(serverClient);
                    }, TaskCreationOptions.LongRunning);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"FATAL ERROR: {e.Message}");
            }
        }

        static async void SendToAllClients(object serverObj)
        {
            await Task.Factory.StartNew(() =>
            {
                lock (_clients)
                {
                    var bf = new BinaryFormatter();
                    foreach (var client in _clients.ToArray())
                    {
                        try
                        {
                            if (client.Client.Connected)
                            {
                                bf.Serialize(client.Client.GetStream(), serverObj);
                            }
                            else
                            {
                                _clients.Remove(client);
                            }
                        }
                        catch (Exception e)
                        {
                            client.Client.Client.Disconnect(false);
                            _clients.Remove(client);
                            WriteExceptionToConsole(e);
                        }
                    }
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
                Console.WriteLine($"{eObj.Client.Login} " +
                    $"[{eObj.Message.SendTime.ToString()}]: " +
                    $"{string.Join(" ", eObj.Message.Content)}");
        }

        private static void WriteSignalAboutConnection(ConnectedClient connectedClient)
        {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"New connection: ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"{connectedClient.Login} - {connectedClient.Source}");
        }
    }
}