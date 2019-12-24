using EncryptedChat.Server;
using EncryptedChat.Server.ClientModel;
using Prism.Commands;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace EncryptedChat.Client.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private TcpClient _tcpClient;
        private NetworkStream _stream;

        public string Host { get; set; } = "192.168.11.1";
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

        public void OnPropertyChanged(string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private void Connect()
        {
            _tcpClient?.Close();

            _tcpClient = new TcpClient();

            try
            {
                _tcpClient.Connect(Host, Port);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Ошибка подключения к серверу", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            _stream = _tcpClient.GetStream();

            _client = new ConnectedClient(GetLocalIPAddress().ToString(), Login);

            Task.Factory.StartNew(() =>
            {
                try
                {
                    var bf = new BinaryFormatter();
                    bf.Serialize(_stream, _client);

                    while (true)
                    {
                        if (_tcpClient.Connected)
                        {
                            var serverObject = bf.Deserialize(_stream);

                            if (serverObject is ConnectedClient conClient)
                            {
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    Messages.Add(new EncryptedObject(new Message($@"Новое подключение: {conClient.Login}", DateTime.Now), conClient));
                                }, DispatcherPriority.Background);
                            }
                            else if (serverObject is EncryptedObject encObj)
                            {
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    Messages.Add(encObj);
                                }, DispatcherPriority.Background);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }, TaskCreationOptions.LongRunning);
        }

        private void SendMessage()
        {
            try
            {
                var message = new EncryptedObject(new Message(Text, DateTime.Now), _client);
                new BinaryFormatter().Serialize(_stream, message);

                Text = string.Empty;
                OnPropertyChanged(nameof(Text));
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Ошибка отправки сообщения", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
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