using Oni2Xml.Serialization;

namespace Oni2Xml.TypeData
{
    class TypeInfo : ISerializable
    {
        public SerializationTypeInfo info;
        public string name;
        public TypeInfo[] subTypes;

        public void Deserialize(IReader reader)
        {
            this.info = (SerializationTypeInfo)reader.ReadByte();
            SerializationTypeInfo typeValue = this.info & SerializationTypeInfo.VALUE_MASK;

            if (typeValue == SerializationTypeInfo.UserDefined || typeValue == SerializationTypeInfo.Enumeration)
            {
                this.name = reader.ReadKleiString();
            }

            if (this.info.HasFlag(SerializationTypeInfo.IS_GENERIC_TYPE))
            {
                byte numTypeArgs = reader.ReadByte();
                this.subTypes = new TypeInfo[numTypeArgs];
                for (int index = 0; index < (int)numTypeArgs; ++index)
                {
                    var subType = new TypeInfo();
                    subType.Deserialize(reader);
                    this.subTypes[index] = subType;
                }
            }
            else
            {
                switch (typeValue)
                {
                    case SerializationTypeInfo.Array:
                        var subType = new TypeInfo();
                        subType.Deserialize(reader);
                        this.subTypes = new TypeInfo[] { subType };
                        break;
                }
            }
        }

        public void Serialize(IWriter writer)
        {
            writer.WriteByte((byte)this.info);
            SerializationTypeInfo typeValue = this.info & SerializationTypeInfo.VALUE_MASK;

            if (typeValue == SerializationTypeInfo.UserDefined || typeValue == SerializationTypeInfo.Enumeration)
            {
                writer.WriteKleiString(this.name);
            }

            if (this.info.HasFlag(SerializationTypeInfo.IS_GENERIC_TYPE))
            {
                writer.WriteByte((byte)this.subTypes.Length);
                foreach(var subType in this.subTypes)
                {
                    subType.Serialize(writer);
                }
            }
            else
            {
                switch (typeValue)
                {
                    case SerializationTypeInfo.Array:
                        this.subTypes[0].Serialize(writer);
                        break;
                }
            }
        }

        public override string ToString()
        {
            return string.Format("TypeInfo {0} ({1})", name, info);
        }
    }
}
