using System;
using System.IO;
using System.Text;

namespace Oni2Xml.Serialization
{
    class BinaryWriter : IWriter
    {
        private MemoryStream stream = new MemoryStream();
        private System.IO.BinaryWriter writer;

        public BinaryWriter()
        {
            writer = new System.IO.BinaryWriter(stream);
        }

        public int Position
        {
            get
            {
                return (int)stream.Position;
            }
        }

        
        public void WriteByte(byte val)
        {
            writer.Write(val);
        }

        public void WriteBytes(byte[] val)
        {
            writer.Write(val);
        }

        public void WriteChars(char[] val)
        {
            writer.Write(val);
        }

        public void WriteDouble(double val)
        {
            writer.Write(val);
        }

        public void WriteInt16(short val)
        {
            writer.Write(val);
        }

        public void WriteInt32(int val)
        {
            writer.Write(val);
        }

        public void WriteInt64(long val)
        {
            writer.Write(val);
        }

        public void WriteKleiString(string val)
        {
            if (val == null)
            {
                writer.Write((int)0);
                return;
            }
            var bytes = Encoding.UTF8.GetBytes(val);
            writer.Write(bytes.Length);
            writer.Write(bytes);
        }

        public void WriteSByte(sbyte val)
        {
            writer.Write(val);
        }

        public void WriteSingle(float val)
        {
            writer.Write(val);
        }

        public void WriteUInt16(ushort val)
        {
            writer.Write(val);
        }

        public void WriteUInt32(uint val)
        {
            writer.Write(val);
        }

        public void WriteUInt64(ulong val)
        {
            writer.Write(val);
        }

        public byte[] GetBytes()
        {
            return stream.ToArray();
        }
    }
}
