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

        public static T DeserializeObject<T>(byte[] obj) where T : class
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                ms.Write(obj, 0, obj.Length);
                ms.Position = 0;
                return formatter.Deserialize(ms) as T;
            }
        }
    }
}