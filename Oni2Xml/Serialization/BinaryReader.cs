using System.Text;

namespace Oni2Xml.Serialization
{
    class BinaryReader : IReader
    {
        private int idx;
        private byte[] bytes;

        public bool IsFinished
        {
            get
            {
                if (this.bytes != null)
                    return this.idx == this.bytes.Length;
                return true;
            }
        }

        public int Position
        {
            get
            {
                return this.idx;
            }
            set
            {
                this.idx = value;
            }
        }

        public BinaryReader(byte[] bytes)
        {
            this.bytes = bytes;
        }

        public unsafe byte ReadByte()
        {
            fixed (byte* numPtr = &this.bytes[this.idx])
            {
                byte num = *numPtr;
                ++this.idx;
                return num;
            }
        }

        public unsafe sbyte ReadSByte()
        {
            fixed (byte* numPtr = &this.bytes[this.idx])
            {
                sbyte num = (sbyte)*numPtr;
                ++this.idx;
                return num;
            }
        }

        public unsafe ushort ReadUInt16()
        {
            fixed (byte* numPtr = &this.bytes[this.idx])
            {
                ushort num = *(ushort*)numPtr;
                this.idx += 2;
                return num;
            }
        }

        public unsafe short ReadInt16()
        {
            fixed (byte* numPtr = &this.bytes[this.idx])
            {
                short num = *(short*)numPtr;
                this.idx += 2;
                return num;
            }
        }

        public unsafe uint ReadUInt32()
        {
            fixed (byte* numPtr = &this.bytes[this.idx])
            {
                uint num = *(uint*)numPtr;
                this.idx += 4;
                return num;
            }
        }

        public unsafe int ReadInt32()
        {
            fixed (byte* numPtr = &this.bytes[this.idx])
            {
                int num = *(int*)numPtr;
                this.idx += 4;
                return num;
            }
        }

        public unsafe ulong ReadUInt64()
        {
            fixed (byte* numPtr = &this.bytes[this.idx])
            {
                ulong num = (ulong)*(long*)numPtr;
                this.idx += 8;
                return num;
            }
        }

        public unsafe long ReadInt64()
        {
            fixed (byte* numPtr = &this.bytes[this.idx])
            {
                long num = *(long*)numPtr;
                this.idx += 8;
                return num;
            }
        }

        public unsafe float ReadSingle()
        {
            fixed (byte* numPtr = &this.bytes[this.idx])
            {
                float num = *(float*)numPtr;
                this.idx += 4;
                return num;
            }
        }

        public unsafe double ReadDouble()
        {
            fixed (byte* numPtr = &this.bytes[this.idx])
            {
                double num = *(double*)numPtr;
                this.idx += 8;
                return num;
            }
        }

        public char[] ReadChars(int length)
        {
            char[] chArray = new char[length];
            for (int index = 0; index < length; ++index)
                chArray[index] = (char)this.bytes[this.idx + index];
            this.idx += length;
            return chArray;
        }

        public byte[] ReadBytes(int length)
        {
            byte[] numArray = new byte[length];
            for (int index = 0; index < length; ++index)
                numArray[index] = this.bytes[this.idx + index];
            this.idx += length;
            return numArray;
        }

        public string ReadKleiString()
        {
            int count = this.ReadInt32();
            string str = (string)null;
            if (count >= 0)
            {
                str = Encoding.UTF8.GetString(this.bytes, this.idx, count);
                this.idx += count;
            }
            return str;
        }

        public void SkipBytes(int length)
        {
            this.idx += length;
        }

        public byte[] RawBytes()
        {
            return this.bytes;
        }
    }

}
