using Oni2Xml.Serialization;
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

            Load(args[0]);
        }

        static OniSaveData Load(string path)
        {
            var bytes = File.ReadAllBytes(path);
            var reader = new Serialization.BinaryReader(bytes);

            var data = new OniSaveData();
            data.Deserialize(reader);
            return data;
        }
    }
}
