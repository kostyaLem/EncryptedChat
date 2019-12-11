using System;

namespace EncryptedChat.Server.ClientModel
{
    [Serializable]
    public class ConnectedClient : BaseClient
    {
        public ConnectedClient(string source, string login)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));
            Login = login ?? throw new ArgumentNullException(nameof(login));
        }

        public virtual string Source { get; }
    }
}