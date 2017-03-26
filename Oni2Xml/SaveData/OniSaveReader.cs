using Oni2Xml.Serialization;
using Oni2Xml.TypeData;

namespace Oni2Xml.SaveData
{
    class OniSaveReader : IOniSaveReader
    {
        private IReader reader;
        private ITypeTemplateRegistry typeRegistry;

        public OniSaveReader(IReader reader, ITypeTemplateRegistry typeRegistry)
        {
            this.reader = reader;
            this.typeRegistry = typeRegistry;
        }

        public ITypeTemplateRegistry TemplateRegistry
        {
            get
            {
                return this.typeRegistry;
            }
        }

        public bool IsFinished
        {
            get
            {
                return this.reader.IsFinished;
            }
        }

        public int Position
        {
            get
            {
                return this.reader.Position;
            }
        }

        public byte[] RawBytes()
        {
            return this.reader.RawBytes();
        }

        public byte ReadByte()
        {
            return this.reader.ReadByte();
        }

        public byte[] ReadBytes(int length)
        {
            return this.reader.ReadBytes(length);
        }

        public char[] ReadChars(int length)
        {
            return this.reader.ReadChars(length);
        }

        public double ReadDouble()
        {
            return this.reader.ReadDouble();
        }

        public short ReadInt16()
        {
            return this.reader.ReadInt16();
        }

        public int ReadInt32()
        {
            return this.reader.ReadInt32();
        }

        public long ReadInt64()
        {
            return this.reader.ReadInt64();
        }

        public string ReadKleiString()
        {
            return this.reader.ReadKleiString();
        }

        public sbyte ReadSByte()
        {
            return this.reader.ReadSByte();
        }

        public float ReadSingle()
        {
            return this.reader.ReadSingle();
        }

        public TypeInstanceData ReadTemplateData(string name)
        {
            return this.typeRegistry.ReadTemplate(name, this);
        }

        public ushort ReadUInt16()
        {
            return this.reader.ReadUInt16();
        }

        public uint ReadUInt32()
        {
            return this.reader.ReadUInt32();
        }

        public ulong ReadUInt64()
        {
            return this.reader.ReadUInt64();
        }

        public void SkipBytes(int length)
        {
            this.reader.SkipBytes(length);
        }
    }
}
