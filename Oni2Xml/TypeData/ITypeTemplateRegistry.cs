
using Oni2Xml.Serialization;

namespace Oni2Xml.TypeData
{
    interface ITypeTemplateRegistry
    {
        bool HasTemplate(string name);

        ObjectInstanceData ReadTemplate(string name, IReader reader);
        void WriteTemplate(ObjectInstanceData data, IWriter writer);
    }
}
