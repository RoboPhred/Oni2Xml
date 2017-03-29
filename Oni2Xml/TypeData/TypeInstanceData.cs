using System.Collections.Generic;

namespace Oni2Xml.TypeData
{
    class TypeInstanceData
    {
    }

    class ObjectInstanceData : TypeInstanceData
    {
        public ObjectInstanceData(string name)
        {
            this.name = name;
        }

        public string name;
        public IDictionary<string, TypeInstanceData> fields = new Dictionary<string, TypeInstanceData>();
        public IDictionary<string, TypeInstanceData> properties = new Dictionary<string, TypeInstanceData>();
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

        // TODO: Remove.  Hack to force key ordering so we can test round trip by shasum.
        public TypeInstanceData[] OrderedKeys;

        public IDictionary<TypeInstanceData, TypeInstanceData> entries;
    }
}
