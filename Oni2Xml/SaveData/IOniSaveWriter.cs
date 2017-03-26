using Oni2Xml.Serialization;
using Oni2Xml.TypeData;

namespace Oni2Xml.SaveData
{
    interface IOniSaveWriter : IWriter
    {
        ITypeTemplateRegistry TemplateRegistry { get; }

        void WriteTemplateData(TypeInstanceData data);
    }
}
