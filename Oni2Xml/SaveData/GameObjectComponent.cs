using Oni2Xml.Serialization;
using Oni2Xml.TypeData;
using System.Diagnostics;

namespace Oni2Xml.SaveData
{
    [DebuggerDisplay("GameObjectComponent {name}")]
    class Component : IOniSaveSerializable
    {
        public string name;
        public ObjectInstanceData saveLoadableData;
        public byte[] saveLoadableDetailsData;

        public void Deserialize(IOniSaveReader reader)
        {
            this.name = reader.ReadKleiString();
            int length = reader.ReadInt32();

            int startPos = reader.Position;

            byte[] data = reader.ReadBytes(length);


            if (reader.Position - startPos != length)
            {
                Debug.WriteLine(string.Format("WARN: Component {0} read differing bytes than length", this.name));
                reader.SkipBytes(length - (reader.Position - startPos));
            }

            if (reader.HasTemplate(this.name))
            {
                var preReadPos = reader.Position;
                this.saveLoadableData = reader.ReadTemplateObject(this.name);
                var bytesRemaining = length - (reader.Position - preReadPos);
                if (bytesRemaining > 0)
                {
                    Debug.WriteLine(string.Format("WARN: Component {0} template did not read all data.  This may be a sign it uses additional parsing (ISaveLoadableDetailJson).", this.name));
                    this.saveLoadableDetailsData = reader.ReadBytes(bytesRemaining);
                }
            }
            else
            {
                Debug.WriteLine(string.Format("WARN: Component {0} has no matching type template", this.name));
                this.saveLoadableDetailsData = data;
            }
        }

        public void Serialize(IOniSaveWriter writer)
        {
            writer.WriteKleiString(this.name);

            // Serialize component data to a new writer, as we need to know the length ahead of writing it.
            var binaryWriter = new BinaryWriter();
            var oniDataWriter = new OniSaveWriter(binaryWriter, writer.TemplateRegistry);

            if (this.saveLoadableData != null)
            {
                if (this.saveLoadableData.name != this.name)
                {
                    throw new System.Exception("Cannot write game object component data template with a different name than that of the component.");
                }
                oniDataWriter.WriteTemplateObject(this.saveLoadableData);
            }
            if (this.saveLoadableDetailsData != null)
            {
                oniDataWriter.WriteBytes(this.saveLoadableDetailsData);
            }

            var data = binaryWriter.GetBytes();

            // Write out the component data.
            writer.WriteInt32(data.Length);
            writer.WriteBytes(data);
        }
    }
}
