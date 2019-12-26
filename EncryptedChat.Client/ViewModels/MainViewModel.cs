using Encrypted;
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
        private RSA _rsa = new RSA();

        private TcpClient _tcpClient;
        private NetworkStream _stream;
        private ConnectedClient _client;

        #region Keys
        public int LocalE => (int)_rsa.e;
        public int LocalN => (int)_rsa.n;

        public int RemoteE { get => remoteE; set { remoteE = value; OnPropertyChanged(); } }
        public int RemoteN { get => remoteN; set { remoteN = value; OnPropertyChanged(); } }
        #endregion

        public string Host { get; set; } = "192.168.11.1";
        public int Port { get; set; } = 5050;
        public string Login { get; set; } = "kostyaLem";
        public string Text { get; set; }

        public bool Connected => RemoteE != 0 && RemoteN != 0 && _canConnect;
        public ObservableCollection<MessageItem> Messages { get; set; }

        private bool _canConnect = true;
        private int remoteE;
        private int remoteN;

        public bool CanDisconnect { get; set; } = false;

        public DelegateCommand SendMessageCommand { get; set; }
        public DelegateCommand ConnectCommand { get; set; }
        public DelegateCommand DisconnectCommand { get; set; }

        public MainViewModel()
        {
            Messages = new ObservableCollection<MessageItem>();

            SendMessageCommand = new DelegateCommand(SendMessage);
            ConnectCommand = new DelegateCommand(Connect);
            DisconnectCommand = new DelegateCommand(Fake);

            Application.Current.MainWindow.Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            Fake();
        }

        private void Fake()
        {
            _tcpClient?.Close();
            _canConnect = true;
            CanDisconnect = false;
            OnPropertyChanged();
        }

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
                                    _canConnect = false;
                                    CanDisconnect = true;
                                    OnPropertyChanged();
                                }, DispatcherPriority.Background);
                            }
                            else if (serverObject is EncryptedObject encObj)
                            {
                                if (encObj.Client.Login == _client.Login)
                                    continue;

                                var text = _rsa.Decrypt(encObj.Message.Content);

                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    Messages.Add(encObj.MapEncObjToMessageItem(text));
                                }, DispatcherPriority.Background);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    _canConnect = true;
                    CanDisconnect = false;
                    OnPropertyChanged();
                }
            }, TaskCreationOptions.LongRunning);
        }

        private void SendMessage()
        {
            try
            {
                var data = _rsa.Encrypt(Text, RemoteE, RemoteN);

                Dispatcher.CurrentDispatcher.Invoke(() =>
                {
                    Messages.Add(new MessageItem() { Login = _client.Login, Content = Text, SendTime = DateTime.Now });
                });

                var message = new EncryptedObject(new Message(data, DateTime.Now), _client);
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


        public void OnPropertyChanged(string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}