using System.Net;
using System.Net.Sockets;
using EncryptedChat.Client.ViewModels;
using System.Windows;
using System;
using EncryptedChat.Client.View;

namespace EncryptedChat.Client
{
    public partial class App : Application
    {
        protected void OnStartup(object sender, StartupEventArgs e)
        {
            MainViewModel mainVM;

            if (e.Args.Length == 2 &&
                IPAddress.TryParse(e.Args[0], out IPAddress host) &&
                int.TryParse(e.Args[1], out int port))
            {                
                mainVM = new MainViewModel(host, port);
            }
            else
            {
                mainVM = new MainViewModel(GetLocalIPAddress(), 5050);
            }
            new MainWindow() { DataContext = mainVM }.ShowDialog();
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