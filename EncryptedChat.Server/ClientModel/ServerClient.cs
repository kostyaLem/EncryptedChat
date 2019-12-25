using System;
using System.Net.Sockets;

namespace EncryptedChat.Server.ClientModel
{
    public class ServerClient : BaseClient
    {
        public ServerClient()
        {

        }

        public ServerClient(TcpClient client, Guid id, string login)
        {            
            Client = client ?? throw new ArgumentNullException(nameof(client));
            Login = login ?? throw new ArgumentNullException(nameof(login));
            ID = id;
        }

        public TcpClient Client { get; }
    }
}