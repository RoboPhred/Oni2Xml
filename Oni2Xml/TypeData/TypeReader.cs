using Oni2Xml.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Oni2Xml.TypeData
{
    class TypeReader
    {


        // TODO: This needs a cleanup to fit into new serializer.

        private IList<TypeTemplate> typeTemplates;

        public TypeReader(IList<TypeTemplate> typeTemplates)
        {
            this.typeTemplates = typeTemplates;
        }

        public bool HasTemplate(string name)
        {
            return this.typeTemplates.Any(x => x.name == name);
        }

        public ObjectInstanceData ReadTemplateObject(string name, IReader reader)
        {
            TypeTemplate template = this.typeTemplates.FirstOrDefault(x => x.name == name);
            if (template == null)
            {
                throw new Exception("Could not find type template for " + name);
            }

            var data = new ObjectInstanceData(template.name);

            foreach (var member in template.fields)
            {
                var value = ReadValue(member.typeInfo, reader);
                if (value != null)
                {
                    data.fields.Add(member.name, value);
                }
            }

            foreach (var member in template.properties)
            {
                var value = ReadValue(member.typeInfo, reader);
                if (value != null)
                {
                    data.properties.Add(member.name, value);
                }
            }

            return data;
        }

        public void WriteTemplateObject(ObjectInstanceData data, IWriter writer)
        {
            var template = this.typeTemplates.FirstOrDefault(x => x.name == data.name);
            if (template == null)
            {
                throw new Exception(string.Format("Template object refers to unknown template name {0}.", data.name));
            }

            foreach(var member in template.fields)
            {
                TypeInstanceData value;
                data.fields.TryGetValue(member.name, out value);
                WriteValue(member.typeInfo, value, writer);
            }

            foreach (var member in template.properties)
            {
                TypeInstanceData value;
                data.properties.TryGetValue(member.name, out value);
                WriteValue(member.typeInfo, value, writer);
            }
        }

        private TypeInstanceData ReadValue(TypeInfo info, IReader reader)
        {
            try
            {
                var valueType = info.info & SerializationTypeInfo.VALUE_MASK;
                switch (valueType)
                {
                    case SerializationTypeInfo.UserDefined:
                        if (reader.ReadInt32() >= 0)
                        {
                            return ReadTemplateObject(info.name, reader);
                        }
                        break;
                    case SerializationTypeInfo.SByte:
                        return new PrimitiveInstanceData(reader.ReadSByte());
                    case SerializationTypeInfo.Byte:
                        return new PrimitiveInstanceData(reader.ReadByte());
                    case SerializationTypeInfo.Boolean:
                        return new PrimitiveInstanceData(reader.ReadByte() == 1);
                    case SerializationTypeInfo.Int16:
                        return new PrimitiveInstanceData(reader.ReadInt16());
                    case SerializationTypeInfo.UInt16:
                        return new PrimitiveInstanceData(reader.ReadUInt16());
                    case SerializationTypeInfo.Int32:
                        return new PrimitiveInstanceData(reader.ReadInt32());
                    case SerializationTypeInfo.UInt32:
                        return new PrimitiveInstanceData(reader.ReadUInt32());
                    case SerializationTypeInfo.Int64:
                        return new PrimitiveInstanceData(reader.ReadInt64());
                    case SerializationTypeInfo.UInt64:
                        return new PrimitiveInstanceData(reader.ReadUInt64());
                    case SerializationTypeInfo.Single:
                        return new PrimitiveInstanceData(reader.ReadSingle());
                    case SerializationTypeInfo.Double:
                        return new PrimitiveInstanceData(reader.ReadDouble());
                    case SerializationTypeInfo.String:
                        return new PrimitiveInstanceData(reader.ReadKleiString());
                    case SerializationTypeInfo.Enumeration:
                        return new PrimitiveInstanceData(reader.ReadUInt32());
                    case SerializationTypeInfo.Vector2I:
                        return new PrimitiveInstanceData(reader.ReadVector2I());
                    case SerializationTypeInfo.Vector2:
                        return new PrimitiveInstanceData(reader.ReadVector2());
                    case SerializationTypeInfo.Vector3:
                        return new PrimitiveInstanceData(reader.ReadVector3());
                    case SerializationTypeInfo.Array:
                    case SerializationTypeInfo.List:
                    case SerializationTypeInfo.HashSet:
                        {
                            var dataLength = reader.ReadInt32();
                            var subtype = info.subTypes[0];
                            int length = reader.ReadInt32();
                            if (length >= 0)
                            {
                                var data = new ArrayInstanceData();
                                for (int index = 0; index < length; ++index)
                                {
                                    data.values.Add(ReadValue(subtype, reader));
                                }
                                return data;
                            }
                        }
                        break;
                    case SerializationTypeInfo.Pair:
                        if (reader.ReadInt32() >= 0)
                        {
                            var key = ReadValue(info.subTypes[0], reader);
                            var value = ReadValue(info.subTypes[1], reader);
                            return new PairInstanceData(key, value);
                        }
                        break;
                    case SerializationTypeInfo.Dictionary:
                        {
                            int dataLength = reader.ReadInt32();
                            int numEntries = reader.ReadInt32();
                            if (numEntries >= 0)
                            {
                                var keyTypeInfo = info.subTypes[0];
                                var keys = new TypeInstanceData[numEntries];

                                var valueTypeInfo = info.subTypes[1];
                                var values = new TypeInstanceData[numEntries];

                                // Values are read first, then keys.
                                for (var i = 0; i < numEntries; i++)
                                {
                                    values[i] = ReadValue(valueTypeInfo, reader);
                                }

                                for (var i = 0; i < numEntries; i++)
                                {
                                    keys[i] = ReadValue(keyTypeInfo, reader);
                                }


                                var data = new DictionaryInstanceData();
                                data.OrderedKeys = keys;
                                for (var i = 0; i < numEntries; i++)
                                {
                                    data.entries.Add(keys[i], values[i]);
                                }
                                return data;
                            }
                        }
                        break;
                    case SerializationTypeInfo.Colour:
                        return new PrimitiveInstanceData(reader.ReadColour());
                    default:
                        throw new Exception(string.Format("Unknown valueType {1}", valueType));
                }
            }
            catch(Exception e)
            {
                throw new Exception(string.Format("While reading {0}: {1}", info, e.Message), e);
            }
            return null;
        }

        private void WriteValue(TypeInfo info, TypeInstanceData data, IWriter writer)
        {
            var valueType = info.info & SerializationTypeInfo.VALUE_MASK;
            switch (valueType)
            {
                case SerializationTypeInfo.UserDefined:
                    if (data != null)
                    {
                        var obj = this.EnsureInstanceType<ObjectInstanceData>(data);
                        var objWriter = new BinaryWriter();
                        this.WriteTemplateObject(obj, objWriter);
                        var bytes = objWriter.GetBytes();
                        writer.WriteInt32(bytes.Length);
                        writer.WriteBytes(bytes);
                    }
                    else
                    {
                        writer.WriteInt32(-1);
                    }
                    break;
                case SerializationTypeInfo.SByte:
                    {
                        if (data == null) return;
                        var val = this.EnsurePrimitive<sbyte>(data);
                        writer.WriteSByte(val);
                    }
                    break;
                case SerializationTypeInfo.Byte:
                    {
                        if (data == null) return;
                        var val = this.EnsurePrimitive<byte>(data);
                        writer.WriteByte(val);
                    }
                    break;
                case SerializationTypeInfo.Boolean:
                    {
                        if (data == null) return;
                        var val = this.EnsurePrimitive<bool>(data);
                        writer.WriteByte((byte)(val ? 1 : 0));
                    }
                    break;
                case SerializationTypeInfo.Int16:
                    {
                        if (data == null) return;
                        var val = this.EnsurePrimitive<short>(data);
                        writer.WriteInt16(val);
                    }
                    break;
                case SerializationTypeInfo.UInt16:
                    {
                        if (data == null) return;
                        var val = this.EnsurePrimitive<ushort>(data);
                        writer.WriteUInt16(val);
                    }
                    break;
                case SerializationTypeInfo.Int32:
                    {
                        if (data == null) return;
                        var val = this.EnsurePrimitive<int>(data);
                        writer.WriteInt32(val);
                    }
                    break;
                case SerializationTypeInfo.UInt32:
                    {
                        if (data == null) return;
                        var val = this.EnsurePrimitive<uint>(data);
                        writer.WriteUInt32(val);
                    }
                    break;
                case SerializationTypeInfo.Int64:
                    {
                        if (data == null) return;
                        var val = this.EnsurePrimitive<long>(data);
                        writer.WriteInt64(val);
                    }
                    break;
                case SerializationTypeInfo.UInt64:
                    {
                        if (data == null) return;
                        var val = this.EnsurePrimitive<ulong>(data);
                        writer.WriteUInt64(val);
                    }
                    break;
                case SerializationTypeInfo.Single:
                    {
                        if (data == null) return;
                        var val = this.EnsurePrimitive<float>(data);
                        writer.WriteSingle(val);
                    }
                    break;
                case SerializationTypeInfo.Double:
                    {
                        if (data == null) return;
                        var val = this.EnsurePrimitive<double>(data);
                        writer.WriteDouble(val);
                    }
                    break;
                case SerializationTypeInfo.String:
                    {
                        if (data == null) return;
                        var val = this.EnsurePrimitive<string>(data);
                        writer.WriteKleiString(val);
                    }
                    break;
                case SerializationTypeInfo.Enumeration:
                    {
                        if (data == null) return;
                        var val = this.EnsurePrimitive<uint>(data);
                        writer.WriteUInt32(val);
                    }
                    break;
                case SerializationTypeInfo.Vector2I:
                    {
                        if (data == null) return;
                        var val = this.EnsurePrimitive<Vector2I>(data);
                        writer.WriteVector2I(val);
                    }
                    break;
                case SerializationTypeInfo.Vector2:
                    {
                        if (data == null) return;
                        var val = this.EnsurePrimitive<Vector2>(data);
                        writer.WriteVector2(val);
                    }
                    break;
                case SerializationTypeInfo.Vector3:
                    {
                        if (data == null) return;
                        var val = this.EnsurePrimitive<Vector3>(data);
                        writer.WriteVector3(val);
                    }
                    break;
                case SerializationTypeInfo.Array:
                case SerializationTypeInfo.List:
                case SerializationTypeInfo.HashSet:
                    if (data == null)
                    {
                        // total length
                        writer.WriteInt32(4);

                        // array length
                        writer.WriteInt32(-1);
                    }
                    else
                    {
                        var arrayWriter = new BinaryWriter();
                        var subtype = info.subTypes[0];
                        var array = this.EnsureInstanceType<ArrayInstanceData>(data);
                        foreach (var value in array.values)
                        {
                            this.WriteValue(subtype, value, arrayWriter);
                        }

                        var bytes = arrayWriter.GetBytes();
                        writer.WriteInt32(bytes.Length);
                        // Count not included in data.
                        writer.WriteInt32(array.values.Count);
                        writer.WriteBytes(bytes);
                    }
                    break;
                case SerializationTypeInfo.Pair:
                    if (data != null)
                    {
                        var pairWriter = new BinaryWriter();
                        var pair = this.EnsureInstanceType<PairInstanceData>(data);
                        this.WriteValue(info.subTypes[0], pair.key, pairWriter);
                        this.WriteValue(info.subTypes[1], pair.value, pairWriter);
                        var bytes = pairWriter.GetBytes();
                        writer.WriteInt32(bytes.Length);
                        writer.WriteBytes(bytes);
                    }
                    else
                    {
                        // NOTE: There must be a bug in klei's code here.
                        //  They write this out if the pair is null,
                        //  yet the pair parser WILL treat it as a valid pair since length is > 0

                        //data length
                        writer.WriteInt32(4);

                        // ??? 
                        writer.WriteInt32(-1);
                    }
                    break;
                case SerializationTypeInfo.Dictionary:
                    if (data == null)
                    {
                        // data length
                        writer.WriteInt32(4);

                        // entry length
                        writer.WriteInt32(-1);
                        return;
                    }
                    else
                    {
                        var dict = this.EnsureInstanceType<DictionaryInstanceData>(data);

                        var dictWriter = new BinaryWriter();
                        // Stores values first, then keys.

                        // TODO: Remove.  Test code to ensure ordering so we can shasum the round trip save for testing.
                        //var entries = dict.entries.ToArray();
                        var entries = dict.OrderedKeys.Select(x => new KeyValuePair<TypeInstanceData, TypeInstanceData>(x, dict.entries[x])).ToArray();

                        var keyTypeInfo = info.subTypes[0];
                        var valueTypeInfo = info.subTypes[1];


                        // Values
                        foreach (var item in entries)
                        {
                            this.WriteValue(valueTypeInfo, item.Value, dictWriter);
                        }

                        // Keys
                        foreach (var item in entries)
                        {
                            this.WriteValue(keyTypeInfo, item.Key, dictWriter);
                        }

                        var bytes = dictWriter.GetBytes();

                        writer.WriteInt32(bytes.Length);
                        // Count not included in data.
                        writer.WriteInt32(dict.entries.Count);
                        writer.WriteBytes(bytes);

                    }
                    break;
                case SerializationTypeInfo.Colour:
                    if (data == null) return;
                    var color = this.EnsurePrimitive<Color>(data);
                    writer.WriteColour(color);
                    break;
                default:
                    throw new Exception(string.Format("Unknown valueType {1}", valueType));
            }
        }

        private T EnsureInstanceType<T>(TypeInstanceData data) where T : TypeInstanceData
        {
            if (data is T == false)
            {
                throw new Exception("Expected instance data of type " + typeof(T).Name);
            }
            return (T)data;
        }

        private T EnsurePrimitive<T>(TypeInstanceData data)
        {
            var prim = EnsureInstanceType<PrimitiveInstanceData>(data);
            if (prim.value != null && prim.value is T == false)
            {
                throw new Exception("Expected primitive value of type " + typeof(T).Name);
            }
            return (T)prim.value;
        }
    }
}
