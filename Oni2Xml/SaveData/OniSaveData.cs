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

        public int versionMajor;
        public int versionMinor;

        public OniSaveDataHeader header = new OniSaveDataHeader();
        public IList<TypeTemplate> templates = new List<TypeTemplate>();
        public IList<GameObjectRoot> gameObjectRoots = new List<GameObjectRoot>();
        public IDictionary<string, ObjectInstanceData> sections = new Dictionary<string, ObjectInstanceData>();

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
            this.sections = new Dictionary<string, ObjectInstanceData>();

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

            // Save versions:
            //  pre-tu: 6.0
            //  tu: 7.1
            if (verMajor != 7 || verMinor > 1)
            {
                throw new Exception(string.Format("SAVE FILE VERSION MISMATCH! Expected {0}.{1} but got {2}.{3}", 7, 1, verMajor, verMinor));
            }

            this.versionMajor = verMajor;
            this.versionMinor = verMinor;

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


            // Settings
            var oniWriter = new OniSaveWriter(writer, new TypeTemplateRegistry(this.templates));

            WriteSection("Klei.SaveFileRoot", this, oniWriter);
            WriteSection("Game+Settings", this, oniWriter);


            // Game State
            writer.WriteChars(SAVE_HEADER);
            writer.WriteInt32(this.versionMajor);
            writer.WriteInt32(this.versionMinor);

            writer.WriteInt32(this.gameObjectRoots.Count);
            foreach(var gameObjectRoot in this.gameObjectRoots)
            {
                gameObjectRoot.Serialize(oniWriter);
            }

            WriteSection("Game+GameSaveData", this, oniWriter);
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
                reader.ReadTemplateObject(sectionName)
            );
        }

        private static void WriteSection(string sectionName, OniSaveData data, IOniSaveWriter writer)
        {
            writer.WriteKleiString(sectionName);
            writer.WriteTemplateObject(data.sections[sectionName]);
        }
    }
}
