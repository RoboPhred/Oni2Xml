using System;
using System.IO;
using System.Text;

namespace Oni2Xml.Serialization
{
    class BinaryWriter : IWriter
    {
        private MemoryStream stream = new MemoryStream();
        private System.IO.BinaryWriter writer;

        private byte[] debugShouldEqual;
        private long debugLastCheck = 0;

        public BinaryWriter(byte[] debugShouldEqual = null)
        {
            this.debugShouldEqual = debugShouldEqual;
            writer = new System.IO.BinaryWriter(stream);
        }

        public int Position
        {
            get
            {
                return (int)stream.Position;
            }
        }

        
        private void TestCheckByte()
        {
            if (debugShouldEqual == null)
            {
                return;
            }

            long length = stream.Position - debugLastCheck;
            var buf = stream.GetBuffer();
            for(var i = 0; i < length; i++)
            {
                var pos = debugLastCheck + i;
                if (buf[i] != this.debugShouldEqual[i])
                {
                    throw new Exception("Data written that does not match.  At " + pos);
                }
            }
            debugLastCheck = stream.Position;
        }

        public void WriteByte(byte val)
        {
            writer.Write(val);
            this.TestCheckByte();
        }

        public void WriteBytes(byte[] val)
        {
            writer.Write(val);
            this.TestCheckByte();
        }

        public void WriteChars(char[] val)
        {
            writer.Write(val);
            this.TestCheckByte();
        }

        public void WriteDouble(double val)
        {
            writer.Write(val);
            this.TestCheckByte();
        }

        public void WriteInt16(short val)
        {
            writer.Write(val);
            this.TestCheckByte();
        }

        public void WriteInt32(int val)
        {
            writer.Write(val);
            this.TestCheckByte();
        }

        public void WriteInt64(long val)
        {
            writer.Write(val);
            this.TestCheckByte();
        }

        public void WriteKleiString(string val)
        {
            var bytes = Encoding.UTF8.GetBytes(val);
            writer.Write(bytes.Length);
            writer.Write(bytes);
            this.TestCheckByte();
        }

        public void WriteSByte(sbyte val)
        {
            writer.Write(val);
            this.TestCheckByte();
        }

        public void WriteSingle(float val)
        {
            writer.Write(val);
            this.TestCheckByte();
        }

        public void WriteUInt16(ushort val)
        {
            writer.Write(val);
            this.TestCheckByte();
        }

        public void WriteUInt32(uint val)
        {
            writer.Write(val);
            this.TestCheckByte();
        }

        public void WriteUInt64(ulong val)
        {
            writer.Write(val);
            this.TestCheckByte();
        }

        public byte[] GetBytes()
        {
            return stream.ToArray();
        }
    }
}
