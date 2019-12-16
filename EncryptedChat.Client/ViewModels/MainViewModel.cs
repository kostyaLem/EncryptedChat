using EncryptedChat.Server;
using Prism.Commands;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace EncryptedChat.Client.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<EncryptedObject> Messages { get; set; }

        public DelegateCommand ConnectCommand { get; set; }

        public MainViewModel()
        {
            Messages = new ObservableCollection<EncryptedObject>();

            Messages.Add(new EncryptedObject(new Message("Hello, all", DateTime.Now), new Server.ClientModel.ConnectedClient("192.168.0.1", "kostyaLem")));
            Messages.Add(new EncryptedObject(new Message("fuck you", DateTime.Now), new Server.ClientModel.ConnectedClient("102.133.0.1", "aza")));

            ConnectCommand = new DelegateCommand(AddMessage);
        }

        private void AddMessage()
        {
            Messages.Add(new EncryptedObject(new Message("fuck you", DateTime.Now), new Server.ClientModel.ConnectedClient("102.133.0.1", "aza")));
        }

        private void OnPropertyChanges(string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}