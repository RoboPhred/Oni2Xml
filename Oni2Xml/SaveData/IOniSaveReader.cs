using Oni2Xml.Serialization;
using Oni2Xml.TypeData;

namespace Oni2Xml.SaveData
{

    interface IOniSaveReader : IReader
    {
        ITypeTemplateRegistry TemplateRegistry { get; }

        ObjectInstanceData ReadTemplateData(string name);
    }

}
