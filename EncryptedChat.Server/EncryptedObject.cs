using EncryptedChat.Server.ClientModel;
using System;

namespace EncryptedChat.Server
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

    public static class Extensions
    {
        public static MessageItem MapEncObjToMessageItem(this EncryptedObject encObj, string text)
        {
            return new MessageItem()
            {
                Content = text,
                Login = encObj.Client.Login,
                SendTime = encObj.Message.SendTime
            };
        }
    }
}