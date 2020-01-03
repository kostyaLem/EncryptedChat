namespace EncryptedChat.Server.ClientModel
{
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