using System;

namespace EncryptedChat.Server.ClientModel
{
    [Serializable]
    public abstract class BaseClient
    {
        public Guid ID { get; protected set; }
        public string Login { get; protected set; }
    }
}
