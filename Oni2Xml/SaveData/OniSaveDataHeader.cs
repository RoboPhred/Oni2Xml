using Oni2Xml.Serialization;
using System.Text;
using Oni2Xml.TypeData;

namespace Oni2Xml.SaveData
{
    class OniSaveDataHeader : ISerializable
    {
        public uint buildVersion;
        public uint headerVersion;
        public string data;

        public void Deserialize(IReader reader)
        {
            this.buildVersion = reader.ReadUInt32();
            var headerSize = reader.ReadInt32();
            this.headerVersion = reader.ReadUInt32();
            this.data = Encoding.UTF8.GetString(reader.ReadBytes(headerSize));
        }

        public void Serialize(IWriter writer)
        {
            var data = Encoding.UTF8.GetBytes(this.data);
            writer.WriteUInt32(this.buildVersion);
            writer.WriteInt32(data.Length);
            writer.WriteUInt32(this.headerVersion);
            writer.WriteBytes(data);
        }
    }
}
