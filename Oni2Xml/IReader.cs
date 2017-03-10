using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oni2Xml
{

    public interface IReader
    {
        int Position { get; }

        bool IsFinished { get; }

        byte ReadByte();

        sbyte ReadSByte();

        short ReadInt16();

        ushort ReadUInt16();

        int ReadInt32();

        uint ReadUInt32();

        long ReadInt64();

        ulong ReadUInt64();

        float ReadSingle();

        double ReadDouble();

        char[] ReadChars(int length);

        byte[] ReadBytes(int length);

        void SkipBytes(int length);

        string ReadKleiString();

        byte[] RawBytes();
    }

}
