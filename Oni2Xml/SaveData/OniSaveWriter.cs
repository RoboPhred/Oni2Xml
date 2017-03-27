using System;
using Oni2Xml.TypeData;
using Oni2Xml.Serialization;

namespace Oni2Xml.SaveData
{
    class OniSaveWriter : IOniSaveWriter
    {
        private IWriter writer;
        private ITypeTemplateRegistry typeRegistry;

        public OniSaveWriter(IWriter writer, ITypeTemplateRegistry typeRegistry)
        {
            this.writer = writer;
            this.typeRegistry = typeRegistry;
        }

        public int Position
        {
            get
            {
                return this.writer.Position;
            }
        }

        public ITypeTemplateRegistry TemplateRegistry
        {
            get
            {
                return this.typeRegistry;
            }
        }

        public void WriteByte(byte val)
        {
            this.writer.WriteByte(val);
        }

        public void WriteBytes(byte[] val)
        {
            this.writer.WriteBytes(val);
        }

        public void WriteChars(char[] val)
        {
            this.writer.WriteChars(val);
        }

        public void WriteDouble(double val)
        {
            this.writer.WriteDouble(val);
        }

        public void WriteInt16(short val)
        {
            this.writer.WriteInt16(val);
        }

        public void WriteInt32(int val)
        {
            this.writer.WriteInt32(val);
        }

        public void WriteInt64(long val)
        {
            this.writer.WriteInt64(val);
        }

        public void WriteKleiString(string val)
        {
            this.writer.WriteKleiString(val);
        }

        public void WriteSByte(sbyte val)
        {
            this.writer.WriteSByte(val);
        }

        public void WriteSingle(float val)
        {
            this.writer.WriteSingle(val);
        }

        public void WriteTemplateData(ObjectInstanceData data)
        {
            this.typeRegistry.WriteTemplate(data, this);
        }

        public void WriteUInt16(ushort val)
        {
            this.writer.WriteUInt16(val);
        }

        public void WriteUInt32(uint val)
        {
            this.writer.WriteUInt32(val);
        }

        public void WriteUInt64(ulong val)
        {
            this.writer.WriteUInt64(val);
        }
    }
}
