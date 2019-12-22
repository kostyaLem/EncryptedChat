using EncryptedChat.Server.ClientModel;
using System;

namespace EncryptedChat.Server
{
	[Serializable]
	public class EncryptedObject
	{
		public EncryptedObject(Message message, ConnectedClient client)
		{
			Message = message ?? throw new ArgumentNullException(nameof(message));
			Client = client ?? throw new ArgumentNullException(nameof(client));
		}

		public Message Message { get; }
		public ConnectedClient Client { get; }
	}

	[Serializable]
	public class Message
	{
		public Message(string content, DateTime sendTime)
		{
			Content = content ?? throw new ArgumentNullException(nameof(content));
			SendTime = sendTime;
		}

		public string Content { get; set; }
		public DateTime SendTime { get; set; }
	}
}