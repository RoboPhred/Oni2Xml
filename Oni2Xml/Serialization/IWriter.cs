namespace Oni2Xml.Serialization
{
    interface IWriter
    {
        int Position { get; }

        void WriteByte(byte val);

        void WriteSByte(sbyte val);

        void WriteInt16(short val);

        void WriteUInt16(ushort val);

        void WriteInt32(int val);

        void WriteUInt32(uint val);

        void WriteInt64(long val);

        void WriteUInt64(ulong val);

        void WriteSingle(float val);

        void WriteDouble(double val);

        void WriteChars(char[] val);

        void WriteBytes(byte[] val);

        void WriteKleiString(string val);
    }
}
