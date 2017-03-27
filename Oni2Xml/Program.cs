using Newtonsoft.Json;
using Oni2Xml.SaveData;
using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

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
            //File.WriteAllText(args[0] + ".json", JsonConvert.SerializeObject(saveData.gameObjectRoots, Formatting.Indented));

            //TestModifyDups(saveData);

            Save(saveData, args[0] + ".test");
        }

        static OniSaveData Load(string path)
        {
            var bytes = File.ReadAllBytes(path);
            var reader = new Serialization.BinaryReader(bytes);

            var data = new OniSaveData();
            data.Deserialize(reader);
            return data;
        }

        static void TestModifyDups(OniSaveData data)
        {
            var minions = data.gameObjectRoots.FirstOrDefault(x => x.tag == "Minion");
            if (minions == null) return;
            var nameIndex = 1;
            foreach(var minion in minions.gameObjects)
            {
                minion.scale.x = 0.5f * nameIndex;
                minion.scale.y = 0.5f * nameIndex;
                minion.scale.z = 0.5f * nameIndex;
                var ident = minion.components.FirstOrDefault(x => x.name == "MinionIdentity");
                if (ident == null) continue;
                ident.saveLoadableData.fields["name"] = new TypeData.PrimitiveInstanceData("TestMinion" + nameIndex++);
            }
        }

        static void Save(OniSaveData data, string path, byte[] ensureEquals = null)
        {
            var writer = new Serialization.BinaryWriter();
            data.Serialize(writer);
            var bytes = writer.GetBytes();

            if (ensureEquals != null)
            {
                for(var i = 0; i < Math.Min(ensureEquals.Length, bytes.Length); i++)
                {
                    if (bytes[i] != ensureEquals[i])
                    {
                        throw new Exception("Round trip mismatch at byte " + i);
                    }
                }
                if (ensureEquals.Length != bytes.Length)
                {
                    throw new Exception("Round trip mismatch: lengths differ");
                }
            }
            File.WriteAllBytes(path, bytes);
        }
    }
}
