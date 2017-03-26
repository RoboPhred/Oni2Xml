using Oni2Xml.Readers;
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

        static void Load(string path)
        {
            var bytes = File.ReadAllBytes(path);
            var reader = new FastReader(bytes);

            var data = SaveData.SaveData.Parse(reader);
        }
    }
}
