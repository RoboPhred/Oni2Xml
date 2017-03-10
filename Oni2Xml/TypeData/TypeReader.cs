using System;
using System.Collections.Generic;
using System.Linq;

namespace Oni2Xml.TypeData
{
    class TypeReader
    {
        private IList<TypeTemplate> typeTemplates;

        public TypeReader(IList<TypeTemplate> typeTemplates)
        {
            this.typeTemplates = typeTemplates;
        }

        public ObjectTemplateData ReadTemplateObject(string name, IReader reader)
        {
            TypeTemplate template = this.typeTemplates.FirstOrDefault(x => x.name == name);
            if (template == null)
            {
                throw new Exception("Could not find type template for " + name);
            }

            var data = new ObjectTemplateData(template);

            foreach(var member in template.members)
            {
                var value = ReadValue(member.typeInfo, reader);
                if (value != null)
                {
                    data.members.Add(member.name, value);
                }
            }

            return data;
        }

        private TypeInstanceData ReadValue(TypeInfo info, IReader reader)
        {
            var valueType = info.info & SerializationTypeInfo.VALUE_MASK;
            switch (valueType)
            {
                case SerializationTypeInfo.UserDefined:
                    if (reader.ReadInt32() >= 0)
                    {
                        var data = ReadTemplateObject(info.name, reader);
                        return new ObjectInstanceData(info, data);
                    }
                    break;
                case SerializationTypeInfo.SByte:
                    return new PrimitiveInstanceData(info, reader.ReadSByte());
                case SerializationTypeInfo.Byte:
                    return new PrimitiveInstanceData(info, reader.ReadByte());
                case SerializationTypeInfo.Boolean:
                    return new PrimitiveInstanceData(info, reader.ReadByte() == 1);
                case SerializationTypeInfo.Int16:
                    return new PrimitiveInstanceData(info, reader.ReadInt16());
                case SerializationTypeInfo.UInt16:
                    return new PrimitiveInstanceData(info, reader.ReadUInt16());
                case SerializationTypeInfo.Int32:
                    return new PrimitiveInstanceData(info, reader.ReadInt32());
                case SerializationTypeInfo.UInt32:
                    return new PrimitiveInstanceData(info, reader.ReadUInt32());
                case SerializationTypeInfo.Int64:
                    return new PrimitiveInstanceData(info, reader.ReadInt64());
                case SerializationTypeInfo.UInt64:
                    return new PrimitiveInstanceData(info, reader.ReadUInt64());
                case SerializationTypeInfo.Single:
                    return new PrimitiveInstanceData(info, reader.ReadSingle());
                case SerializationTypeInfo.Double:
                    return new PrimitiveInstanceData(info, reader.ReadDouble());
                case SerializationTypeInfo.String:
                    return new PrimitiveInstanceData(info, reader.ReadKleiString());
                case SerializationTypeInfo.Enumeration:
                    return new PrimitiveInstanceData(info, reader.ReadUInt32());
                case SerializationTypeInfo.Vector2I:
                    return new PrimitiveInstanceData(info, reader.ReadVector2I());
                case SerializationTypeInfo.Vector2:
                    return new PrimitiveInstanceData(info, reader.ReadVector2());
                case SerializationTypeInfo.Vector3:
                    return new PrimitiveInstanceData(info, reader.ReadVector3());
                case SerializationTypeInfo.Array:
                case SerializationTypeInfo.List:
                case SerializationTypeInfo.HashSet:
                    reader.ReadInt32();
                    var subtype = info.subTypes[0];
                    int length = reader.ReadInt32();
                    if (length >= 0)
                    {
                        var data = new ArrayInstanceData(info);
                        for (int index = 0; index < length; ++index)
                        {
                            data.values.Add(ReadValue(subtype, reader));
                        }
                        return data;
                    }
                    break;
                case SerializationTypeInfo.Pair:
                    if (reader.ReadInt32() >= 0)
                    {
                        var key = ReadValue(info.subTypes[0], reader);
                        var value = ReadValue(info.subTypes[1], reader);
                        return new PairInstanceData(info, key, value);
                    }
                    break;
                case SerializationTypeInfo.Dictionary:
                    reader.ReadInt32();
                    int numEntries = reader.ReadInt32();
                    if (numEntries >= 0)
                    {
                        var keyTypeInfo = info.subTypes[0];
                        var keys = new TypeInstanceData[numEntries];

                        var valueTypeInfo = info.subTypes[1];
                        var values = new TypeInstanceData[numEntries];

                        // Values are read first, then keys.
                        for(var i = 0; i < numEntries; i++)
                        {
                            values[i] = ReadValue(valueTypeInfo, reader);
                        }

                        for(var i = 0; i < numEntries; i++)
                        {
                            keys[i] = ReadValue(keyTypeInfo, reader);
                        }


                        var data = new DictionaryInstanceData(info);
                        for(var i = 0; i < numEntries; i++)
                        {
                            data.entries.Add(keys[i], values[i]);
                        }
                        return data;
                    }
                    break;
                case SerializationTypeInfo.Colour:
                    return new PrimitiveInstanceData(info, reader.ReadColour());
                default:
                    throw new ArgumentException("unknown type");
            }
            return null;
        }
    }
}
