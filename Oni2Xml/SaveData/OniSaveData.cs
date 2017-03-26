using Oni2Xml.Serialization;
using Oni2Xml.TypeData;
using System;
using System.Collections.Generic;

namespace Oni2Xml.SaveData
{
    class OniSaveData : ISerializable
    {
        private static readonly char[] SAVE_HEADER = new char[4] {
            'K',
            'S',
            'A',
            'V'
        };

        public OniSaveDataHeader header = new OniSaveDataHeader();
        public IList<TypeTemplate> templates = new List<TypeTemplate>();
        public IList<GameObjectRoot> gameObjectRoots = new List<GameObjectRoot>();
        public IDictionary<string, TypeInstanceData> sections = new Dictionary<string, TypeInstanceData>();

        public void Deserialize(IReader reader)
        {
            // Header
            this.header = new OniSaveDataHeader();
            this.header.Deserialize(reader);


            // Templates
            this.templates = new List<TypeTemplate>();
            var numDeserializationTemplates = reader.ReadInt32();
            for (var i = 0; i < numDeserializationTemplates; i++)
            {
                var template = new TypeTemplate();
                template.Deserialize(reader);
                templates.Add(template);
            }


            // "world" string
            var worldHeader = reader.ReadKleiString();
            if (worldHeader != "world")
            {
                throw new Exception("Invalid world header.  Expected header string to be \"world\", but got " + worldHeader.Substring(0, 100));
            }


            // Settings
            this.sections = new Dictionary<string, TypeInstanceData>();

            var oniReader = new OniSaveReader(reader, new TypeTemplateRegistry(this.templates));

            LoadSection("Klei.SaveFileRoot", this, oniReader);
            LoadSection("Game+Settings", this, oniReader);


            // Game State
            char[] chArray = reader.ReadChars(SAVE_HEADER.Length);
            if (chArray == null || chArray.Length != SAVE_HEADER.Length)
                throw new Exception("ManagerHeader length mismatch");
            for (int index = 0; index < SAVE_HEADER.Length; ++index)
            {
                if ((int)chArray[index] != (int)SAVE_HEADER[index])
                    throw new Exception("ManagerHeader content mismatch");
            }
            int verMajor = reader.ReadInt32();
            int verMinor = reader.ReadInt32();
            if (verMajor != 6 || verMinor > 0)
            {
                throw new Exception(string.Format("SAVE FILE VERSION MISMATCH! Expected {0}.{1} but got {2}.{3}", 6, 0, verMajor, verMinor));
            }

            int numPrefabs = reader.ReadInt32();
            for (var i = 0; i < numPrefabs; i++)
            {
                var root = new GameObjectRoot();
                root.Deserialize(oniReader);
                this.gameObjectRoots.Add(root);
            }

            LoadSection("Game+GameSaveData", this, oniReader);
        }

        public void Serialize(IWriter writer)
        {
            // Header
            this.header.Serialize(writer);


            // Templates
            writer.WriteInt32(this.templates.Count);
            foreach (var template in this.templates)
            {
                template.Serialize(writer);
            }


            // "world" string
            writer.WriteKleiString("world");


        }

        private static void LoadSection(string sectionName, OniSaveData data, IOniSaveReader reader)
        {
            var rootName = reader.ReadKleiString();
            if (rootName != sectionName)
            {
                throw new Exception(string.Format("Expected data object named {0} but got {1}", sectionName, rootName.Substring(1000)));
            }

            data.sections.Add(
                sectionName,
                reader.ReadTemplateData(sectionName)
            );
        }
    }
}
