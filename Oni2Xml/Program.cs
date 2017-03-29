using Newtonsoft.Json;
using Oni2Xml.SaveData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Oni2Xml
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.Error.WriteLine("Usage: <export-objects | import-objects>");
                Environment.Exit(1);
                return;
            }

            try
            {
                switch (args[0])
                {
                    case "export-objects":
                        ExportObjectsCmd(args);
                        break;
                    case "import-objects":
                        ImportObjectsCmd(args);
                        break;
                    default:
                        throw new Exception(string.Format("Unknown command {0}", args[0]));
                }
            }
            catch(Exception e)
            {
                if (System.Diagnostics.Debugger.IsAttached)
                    throw;
                Console.Error.WriteLine("Error processing command:\n" + e.Message);
                Console.Error.WriteLine(e.StackTrace);
                Environment.Exit(1);
                return;
            }
        }


        static void ExportObjectsCmd(string[] args)
        {
            string savePath;
            string exportPath;
            if (args.Length < 2)
            {
                Console.Error.WriteLine("Usage: export-objects <save file> [export path]");
                Environment.Exit(1);
                return;
            }

            savePath = args[1];

            if (args.Length >= 3)
            {
                exportPath = args[2];
            }
            else
            {
                exportPath = Path.GetFileNameWithoutExtension(savePath) + ".gameObjects.json";
            }

            var saveData = LoadOniSave(savePath);

            var serializer = new JsonSerializer();
            // Hack until we switch to something more sane.
            //  This is where xml will be useful, as element names or attributes can represent the data type.

            var json = JsonConvert.SerializeObject(saveData.gameObjectRoots, new JsonSerializerSettings()
            {
                TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple,
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented
            });

            File.WriteAllText(exportPath, json);
        }

        static void ImportObjectsCmd(string[] args)
        {
            string gameObjectPath;
            string savePath;
            if (args.Length < 2)
            {
                Console.Error.WriteLine("Usage: import-objects <game objects json> <save path>");
                Environment.Exit(1);
                return;
            }

            gameObjectPath = args[1];

            if (args.Length >= 3)
            {
                savePath = args[2];
            }
            else
            {
                savePath = Path.GetFileNameWithoutExtension(gameObjectPath) + ".gameObjects.json";
            }

            var json = File.ReadAllText(gameObjectPath);

            var saveData = LoadOniSave(savePath);

            saveData.gameObjectRoots = JsonConvert.DeserializeObject<List<GameObjectRoot>>(json, new JsonSerializerSettings()
            {
                TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple,
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented
            });

            WriteOniSave(saveData, savePath);
        }

        static OniSaveData LoadOniSave(string path)
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

        static void WriteOniSave(OniSaveData data, string path)
        {
            var writer = new Serialization.BinaryWriter();
            data.Serialize(writer);
            var bytes = writer.GetBytes();
            File.WriteAllBytes(path, bytes);
        }
    }
}
