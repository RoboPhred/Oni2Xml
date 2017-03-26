using Oni2Xml.Serialization;
using System.Collections.Generic;
using System.Diagnostics;

namespace Oni2Xml.TypeData
{
    [DebuggerDisplay("TypeTemplate {name}")]
    class TypeTemplate : ISerializable
    {
        public string name;
        public List<TypeField> fields = new List<TypeField>();
        public List<TypeField> properties = new List<TypeField>();


        public void Deserialize(IReader reader)
        {
            this.name = reader.ReadKleiString();

            var numFields = reader.ReadInt32();
            var numProperties = reader.ReadInt32();

            for (var i = 0; i < numFields; i++)
            {
                var name = reader.ReadKleiString();
                var typeInfo = new TypeInfo();
                typeInfo.Deserialize(reader);
                this.fields.Add(new TypeField
                {
                    name = name,
                    typeInfo = typeInfo
                });
            }

            for (var i = 0; i < numProperties; i++)
            {
                var name = reader.ReadKleiString();
                var typeInfo = new TypeInfo();
                typeInfo.Deserialize(reader);
                this.properties.Add(new TypeField
                {
                    name = name,
                    typeInfo = typeInfo
                });
            }
        }

        public void Serialize(IWriter writer)
        {
            writer.WriteKleiString(this.name);

            writer.WriteInt32(this.fields.Count);
            writer.WriteInt32(this.properties.Count);

            foreach(var field in this.fields)
            {
                writer.WriteKleiString(field.name);
                field.typeInfo.Serialize(writer);
            }

            foreach(var prop in this.properties)
            {
                writer.WriteKleiString(prop.name);
                prop.typeInfo.Serialize(writer);
            }
        }
    }

    [DebuggerDisplay("TypeField {name})")]
    struct TypeField
    {
        public string name;
        public TypeInfo typeInfo;
    }
}
