
using Oni2Xml.Serialization;

namespace Oni2Xml.TypeData
{
    interface ITypeTemplateRegistry
    {
        bool HasTemplate(string name);

        TypeInstanceData ReadTemplate(string name, IReader reader);
        void WriteTemplate(TypeInstanceData data, IWriter writer);
    }
}
