using Oni2Xml.SaveData;
using System;
using System.IO;

namespace Oni2Xml
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.Error.WriteLine("Usage: <save file> <output file>");
                Environment.Exit(1);
                return;
            }

            var bytes = File.ReadAllBytes(args[0]);
            var saveData = Load(args[0]);
            Save(saveData, args[0] + ".test", bytes);
        }

        static OniSaveData Load(string path)
        {
            var bytes = File.ReadAllBytes(path);
            var reader = new Serialization.BinaryReader(bytes);

            var data = new OniSaveData();
            data.Deserialize(reader);
            return data;
        }

        static void Save(OniSaveData data, string path, byte[] original = null)
        {
            var writer = new Serialization.BinaryWriter();
            data.Serialize(writer);
            var bytes = writer.GetBytes();

            if (original != null)
            {
                for(var i = 0; i < Math.Min(original.Length, bytes.Length); i++)
                {
                    if (bytes[i] != original[i])
                    {
                        throw new Exception("Round trip mismatch at byte " + i);
                    }
                }
                if (original.Length != bytes.Length)
                {
                    throw new Exception("Round trip mismatch: lengths differ");
                }
            }
            File.WriteAllBytes(path, bytes);
        }
    }
}
