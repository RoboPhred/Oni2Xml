using System;
using System.Collections.Generic;

namespace Oni2Xml.SaveData
{
    class GameObject : IOniSaveSerializable
    {
        public Vector3 position;
        public Quaternion rotation;

        public Vector3 scale;

        public byte folder;

        public IList<Component> components = new List<Component>();

        public void Deserialize(IOniSaveReader reader)
        {
            this.components = new List<Component>();

            this.position = reader.ReadVector3();
            this.rotation = reader.ReadQuaternion();
            this.scale = reader.ReadVector3();

            this.folder = reader.ReadByte();


            var numComponents = reader.ReadInt32();
            for (var i = 0; i < numComponents; i++)
            {
                var component = new Component();
                component.Deserialize(reader);
                this.components.Add(component);
            }
        }

        public void Serialize(IOniSaveWriter writer)
        {
            writer.WriteVector3(this.position);
            writer.WriteQuaternion(this.rotation);
            writer.WriteVector3(this.scale);

            writer.WriteByte(this.folder);

            writer.WriteInt32(this.components.Count);
            foreach(var component in this.components)
            {
                component.Serialize(writer);
            }
        }
    }
}
