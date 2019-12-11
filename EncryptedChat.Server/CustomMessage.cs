using EncryptedChat.Server.ClientModel;
using System;

namespace EncryptedChat.Server
{
    [Serializable]
    public class CustomMessage
    {
        public CustomMessage(string message, ConnectedClient client)
        {
            Message = message ?? throw new ArgumentNullException(nameof(message));
            Client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public string Message { get; }
        public ConnectedClient Client { get; }
    }
}