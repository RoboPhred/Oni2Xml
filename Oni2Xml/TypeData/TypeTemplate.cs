using System.Collections.Generic;
using System.Diagnostics;

namespace Oni2Xml.TypeData
{
    class TypeTemplate
    {
        public static TypeTemplate Parse(IReader reader)
        {
            var template = new TypeTemplate();

            template.name = reader.ReadKleiString();

            var numFields = reader.ReadInt32();
            var numProperties = reader.ReadInt32();

            for (var i = 0; i < numFields; i++)
            {
                var fieldName = reader.ReadKleiString();
                var typeInfo = TypeInfo.Parse(reader);
                template.members.Add(new TypeField
                {
                    name = fieldName,
                    typeInfo = typeInfo
                });
            }

            for (var i = 0; i < numProperties; i++)
            {
                var propertyName = reader.ReadKleiString();

                var typeInfo = TypeInfo.Parse(reader);
                template.members.Add(new TypeField
                {
                    name = propertyName,
                    typeInfo = typeInfo
                });
            }

            return template;
        }


        public string name;
        public List<TypeField> members = new List<TypeField>();
    }

    struct TypeField
    {
        public string name;
        public TypeInfo typeInfo;
    }
}
