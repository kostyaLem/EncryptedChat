using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace EncryptedChat.Server
{
    public static class Extensions
    {
        public static byte[] SerializeObject<T>(T obj)
        {
            using (var ms = new MemoryStream())
            {
                new BinaryFormatter().Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public static T DeserializeObject<T>(byte[] obj)
        {
            using (var ms = new MemoryStream(obj))
            {
                return (T)new BinaryFormatter().Deserialize(ms);
            }
        }
    }
}