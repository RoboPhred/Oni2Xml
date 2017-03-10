namespace Oni2Xml.TypeData
{
    class TypeInfo
    {
        public static TypeInfo Parse(IReader reader)
        {
            TypeInfo typeInfo = new TypeInfo();

            typeInfo.info = (SerializationTypeInfo)reader.ReadByte();
            SerializationTypeInfo typeValue = typeInfo.info & SerializationTypeInfo.VALUE_MASK;

            if (typeValue == SerializationTypeInfo.UserDefined || typeValue == SerializationTypeInfo.Enumeration)
            {
                typeInfo.name = reader.ReadKleiString();
            }

            if (typeInfo.info.HasFlag(SerializationTypeInfo.IS_GENERIC_TYPE))
            {
                byte numTypeArgs = reader.ReadByte();
                typeInfo.subTypes = new TypeInfo[numTypeArgs];
                for (int index = 0; index < (int)numTypeArgs; ++index)
                {
                    typeInfo.subTypes[index] = Parse(reader);
                }
            }
            else
            {
                switch (typeValue)
                {
                    case SerializationTypeInfo.Array:
                        typeInfo.subTypes = new TypeInfo[] { Parse(reader) };
                        break;
                }
            }

            return typeInfo;
        }

        public SerializationTypeInfo info;
        public string name;
        public TypeInfo[] subTypes;
    }
}
