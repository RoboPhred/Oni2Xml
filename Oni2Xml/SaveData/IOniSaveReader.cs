using Oni2Xml.Serialization;
using Oni2Xml.TypeData;

namespace Oni2Xml.SaveData
{

    interface IOniSaveReader : IReader
    {
        bool HasTemplate(string name);

        ObjectInstanceData ReadTemplateObject(string name);
    }

}
