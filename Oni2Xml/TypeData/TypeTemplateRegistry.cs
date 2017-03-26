using System;
using System.Collections.Generic;
using Oni2Xml.Serialization;

namespace Oni2Xml.TypeData
{
    class TypeTemplateRegistry : ITypeTemplateRegistry
    {
        private TypeReader typeReader;

        public TypeTemplateRegistry(IList<TypeTemplate> templates)
        {
            this.typeReader = new TypeReader(templates);
        }

        public bool HasTemplate(string name)
        {
            return this.typeReader.HasTemplate(name);
        }

        public TypeInstanceData ReadTemplate(string name, IReader reader)
        {
            return this.typeReader.ReadTemplateObject(name, reader);
        }

        public void WriteTemplate(TypeInstanceData data, IWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
