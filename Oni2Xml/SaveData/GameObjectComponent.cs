using Oni2Xml.Serialization;
using Oni2Xml.TypeData;
using System;
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

            if (reader.TemplateRegistry.HasTemplate(this.name))
            {
                //var preReadPos = reader.Position;
                //this.saveLoadData = reader.ReadTemplateData(this.name);
                //var bytesRemaining = length - (reader.Position - preReadPos);
                //if (bytesRemaining > 0)
                //{
                //    this.saveLoadDetailsData = reader.ReadBytes(bytesRemaining);
                //}
                var templateReader = new BinaryReader(data);
                this.saveLoadableData = reader.TemplateRegistry.ReadTemplate(this.name, templateReader);
                if (!templateReader.IsFinished)
                {
                    this.saveLoadableDetailsData = templateReader.ReadBytes(data.Length - templateReader.Position);
                    Debug.WriteLine(string.Format("WARN: Component {0} template did not read all data.  This may be a sign it uses additional parsing (ISaveLoadableDetailJson).", this.name));
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
                oniDataWriter.WriteTemplateData(this.saveLoadableData);
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
