using Newtonsoft.Json;
using System.Text;

namespace Oni2Xml.SaveData
{
    static class HeaderParser
    {

        public static Header Parse(IReader reader)
        {
            var header = new Header();
            header.buildVersion = reader.ReadUInt32();
            header.headerSize = reader.ReadInt32();
            header.headerVersion = reader.ReadUInt32();
            header.data = Encoding.UTF8.GetString(reader.ReadBytes(header.headerSize));
            return header;
        }
    }

    public struct Header
    {
        public uint buildVersion;
        public int headerSize;
        public uint headerVersion;
        public string data;
    }
}
