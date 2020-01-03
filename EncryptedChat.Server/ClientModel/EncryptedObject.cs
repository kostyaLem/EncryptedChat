using System;

namespace EncryptedChat.Server.ClientModel
{
    [Serializable]
    public class EncryptedObject
    {
        public EncryptedObject(Message message, ConnectedClient client)
        {
            Message = message;
            Client = client;
        }

        public Message Message { get; }
        public ConnectedClient Client { get; }
    }

    [Serializable]
    public class Message
    {
        public Message(string[] content, DateTime sendTime)
        {
            Content = content;
            SendTime = sendTime;
        }

        public string[] Content { get; set; }
        public DateTime SendTime { get; set; }
    }

    public class MessageItem
    {
        public string Content { get; set; }
        public DateTime SendTime { get; set; }
        public string Login { get; set; }
    }
}