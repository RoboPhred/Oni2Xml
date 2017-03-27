using Oni2Xml.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Oni2Xml.SaveData
{
    [DebuggerDisplay("GameObectRoot {tag}")]
    class GameObjectRoot : IOniSaveSerializable
    {
        public string tag;
        public IList<GameObject> gameObjects = new List<GameObject>();

        public void Deserialize(IOniSaveReader reader)
        {
            string tag = reader.ReadKleiString();
            int capacity = reader.ReadInt32();
            int length = reader.ReadInt32();

            int startPos = reader.Position;

            this.tag = tag;
            this.gameObjects = new List<GameObject>();

            for (var i = 0; i < capacity; i++)
            {
                var gameObj = new GameObject();
                gameObj.Deserialize(reader);
                this.gameObjects.Add(gameObj);
            }


            if (reader.Position - startPos != length)
            {
                Debug.WriteLine(string.Format("WARN: Prefab {0} read differing bytes than length", tag));
                reader.SkipBytes(length - (reader.Position - startPos));
            }
        }

        public void Serialize(IOniSaveWriter writer)
        {
            writer.WriteKleiString(this.tag);
            writer.WriteInt32(this.gameObjects.Count);

            // Write the objects to a different writer, so we can know their length before writing them out.
            var binaryWriter = new BinaryWriter();
            var oniObjectWriter = new OniSaveWriter(binaryWriter, writer.TemplateRegistry);
            foreach (var gameObject in this.gameObjects) {
                gameObject.Serialize(oniObjectWriter);
            }

            var objBytes = binaryWriter.GetBytes();
            writer.WriteInt32(objBytes.Length);
            writer.WriteBytes(objBytes);
        }
    }
}
