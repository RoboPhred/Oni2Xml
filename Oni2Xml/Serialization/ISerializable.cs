namespace Oni2Xml.Serialization
{
    interface ISerializable
    {
        void Deserialize(IReader reader);
        void Serialize(IWriter writer);
    }
}
