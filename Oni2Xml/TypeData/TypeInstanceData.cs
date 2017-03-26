using System.Collections.Generic;

namespace Oni2Xml.TypeData
{
    class TypeInstanceData
    {
    }

    class ObjectInstanceData : TypeInstanceData
    {
        public ObjectInstanceData(TypeTemplate template)
        {
            this.template = template;
        }

        public TypeTemplate template;
        public IDictionary<string, TypeInstanceData> members = new Dictionary<string, TypeInstanceData>();
    }

    class PrimitiveInstanceData : TypeInstanceData
    {
        public PrimitiveInstanceData(object value)
        {
            this.value = value;
        }

        public object value;
    }

    class ArrayInstanceData : TypeInstanceData
    {
        public ArrayInstanceData()
        {
            this.values = new List<TypeInstanceData>();
        }

        public IList<TypeInstanceData> values;
    }

    class PairInstanceData : TypeInstanceData
    {
        public PairInstanceData(TypeInstanceData key, TypeInstanceData value) 
        {
            this.key = key;
            this.value = value;
        }

        public TypeInstanceData key;
        public TypeInstanceData value;
    }

    class DictionaryInstanceData : TypeInstanceData
    {
        public DictionaryInstanceData()
        {
            this.entries = new Dictionary<TypeInstanceData, TypeInstanceData>();
        }

        public IDictionary<TypeInstanceData, TypeInstanceData> entries;
    }
}
