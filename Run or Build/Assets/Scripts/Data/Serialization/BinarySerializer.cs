using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class BinarySerializer
    {
        public static void Serializer(string path, object data)
        {
            using var stream = new FileStream(path, FileMode.OpenOrCreate);
            var formatter = new BinaryFormatter();
            formatter.Serialize(stream, data);
        }

        public static T Deserializer<T>(string path)
        {
            using var stream = new FileStream(path, FileMode.Open);
            var formatter = new BinaryFormatter();
            var data = (T)formatter.Deserialize(stream);
            return data;
        }
    }
