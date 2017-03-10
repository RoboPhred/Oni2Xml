using System.Collections.Generic;

namespace Oni2Xml.TypeData
{

    class ObjectTemplateData
    {
        public ObjectTemplateData(TypeTemplate template)
        {
            this.template = template;
            this.members = new Dictionary<string, TypeInstanceData>();
        }

        public TypeTemplate template;
        public IDictionary<string, TypeInstanceData> members;
    }

    class TypeInstanceData
    {
        public TypeInstanceData(TypeInfo typeInfo)
        {
            this.typeInfo = typeInfo;
        }

        public TypeInfo typeInfo;
    }

    class ObjectInstanceData : TypeInstanceData
    {
        public ObjectInstanceData(TypeInfo typeInfo, ObjectTemplateData data)
            : base(typeInfo)
        {
            this.data = data;
        }

        public ObjectTemplateData data;
    }

    class PrimitiveInstanceData : TypeInstanceData
    {
        public PrimitiveInstanceData(TypeInfo typeInfo, object value)
            : base(typeInfo)
        {
            this.value = value;
        }

        public object value;
    }

    class ArrayInstanceData : TypeInstanceData
    {
        public ArrayInstanceData(TypeInfo typeInfo)
            : base(typeInfo)
        {
            this.values = new List<TypeInstanceData>();
        }

        public IList<TypeInstanceData> values;
    }

    class PairInstanceData : TypeInstanceData
    {
        public PairInstanceData(TypeInfo typeInfo, TypeInstanceData key, TypeInstanceData value) 
            : base(typeInfo)
        {
            this.key = key;
            this.value = value;
        }

        public TypeInstanceData key;
        public TypeInstanceData value;
    }

    class DictionaryInstanceData : TypeInstanceData
    {
        public DictionaryInstanceData(TypeInfo typeInfo)
            : base(typeInfo)
        {
            this.entries = new Dictionary<TypeInstanceData, TypeInstanceData>();
        }

        public IDictionary<TypeInstanceData, TypeInstanceData> entries;
    }
}
