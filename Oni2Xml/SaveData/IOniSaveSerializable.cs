using Oni2Xml.TypeData;

namespace Oni2Xml.SaveData
{
    interface IOniSaveSerializable
    {
        void Deserialize(IOniSaveReader reader);
        void Serialize(IOniSaveWriter writer);
    }
}
