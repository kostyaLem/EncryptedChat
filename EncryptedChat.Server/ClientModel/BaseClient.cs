using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncryptedChat.Server.ClientModel
{
    [Serializable]
    public abstract class BaseClient
    {
        public Guid ID { get; protected set; }
        public string Login { get; protected set; }
    }
}
