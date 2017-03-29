
using Oni2Xml.Serialization;

namespace Oni2Xml.TypeData
{
    interface ITypeTemplateRegistry
    {
        bool HasTemplate(string name);

        ObjectInstanceData ReadTemplateObject(string name, IReader reader);
        void WriteTemplateObject(ObjectInstanceData data, IWriter writer);
    }
}
