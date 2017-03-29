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

        static void Save(OniSaveData data, string path)
        {
            var writer = new Serialization.BinaryWriter();
            data.Serialize(writer);
            var bytes = writer.GetBytes();
            File.WriteAllBytes(path, bytes);
        }
    }
}
