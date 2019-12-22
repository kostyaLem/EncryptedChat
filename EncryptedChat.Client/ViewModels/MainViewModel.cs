using EncryptedChat.Server;
using EncryptedChat.Server.ClientModel;
using Prism.Commands;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace EncryptedChat.Client.ViewModels
{
    public class MainViewModel
    {
        private TcpClient _tcpClient;
        private NetworkStream _stream;

        public string Host { get; set; } = "192.168.0.60";
        public int Port { get; set; } = 5050;
        public string Login { get; set; } = "kostyaLem";
        public string Text { get; set; }

        public bool Connected { get; set; }
        public ObservableCollection<EncryptedObject> Messages { get; set; }

        public DelegateCommand SendMessageCommand { get; set; }
        public DelegateCommand ConnectCommand { get; set; }
        public DelegateCommand DisconnectCommand { get; set; }

        public MainViewModel()
        {
            Messages = new ObservableCollection<EncryptedObject>();

            SendMessageCommand = new DelegateCommand(SendMessage);
            ConnectCommand = new DelegateCommand(Connect);
            DisconnectCommand = new DelegateCommand(() => { });
        }

        private ConnectedClient _client;
        private void Connect()
        {
            _tcpClient = new TcpClient();
            _tcpClient.Connect(Host, Port);
            _stream = _tcpClient.GetStream();            

            _client = new ConnectedClient(GetLocalIPAddress().ToString(), Login);

            using (var ms = new MemoryStream(Extensions.SerializeObject(_client)))
            {
                var bytes = new byte[ms.Length];
                ms.Read(bytes, 0, bytes.Length);
                _stream.Write(bytes, 0, bytes.Length);
            }
        }

        private void SendMessage()
        {
            var message = new EncryptedObject(new Message(Text, DateTime.Now), _client);

            using (var ms = new MemoryStream(Extensions.SerializeObject(message)))
            {
                var buffer = new byte[ms.Length];
                ms.Read(buffer, 0, buffer.Length);
                _stream.Write(buffer, 0, buffer.Length);
            }

            Text = string.Empty;
        }

        private static IPAddress GetLocalIPAddress()
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
    }
}