using System;
using System.Collections.Generic;

namespace Oni2Xml.SaveData
{
    static class PrefabParser
    {


        public static PrefabData Parse(IReader reader)
        {

            var prefabs = new PrefabData()
            {
                prefabs = new List<Prefab>()
            };

            int numPrefabs = reader.ReadInt32();
            for (var i = 0; i< numPrefabs; i++)
            {
                var prefab = ParsePrefab(reader);
                prefabs.prefabs.Add(prefab);
            }

            return prefabs;
        }

        private static Prefab ParsePrefab(IReader reader)
        {
            string name = reader.ReadKleiString();
            int capacity = reader.ReadInt32();
            int length = reader.ReadInt32();

            var prefab = new Prefab()
            {
                tag = name,
                capacity = capacity,
                length = length,
                Components = new List<Component>()
            };

            prefab.position = reader.ReadVector3();
            prefab.rotation = reader.ReadQuaternion();
            prefab.scale = reader.ReadVector3();

            prefab.folder = reader.ReadByte();


            var numComponents = reader.ReadInt32();
            for(var i = 0; i < numComponents; i++)
            {
                var component = ParseComponent(reader);
                prefab.Components.Add(component);
            }
            
            return prefab;
        }

        private static Component ParseComponent(IReader reader)
        {
            string key = reader.ReadKleiString();
            int length = reader.ReadInt32();
            byte[] data = reader.ReadBytes(length);

            var component = new Component()
            {
                name = key,
                rawData = data
            };

            // TODO: Identify mono behaviors from key and try to make sense of their data.

            return component;
        }
    }

    struct PrefabData
    {
        public IList<Prefab> prefabs;
    }

    class Prefab
    {
        public string tag;
        public int capacity;
        public int length;

        public Vector3 position;
        public Quaternion rotation;

        public Vector3 scale;

        public byte folder;

        public IList<Component> Components;
    }

    struct Component
    {
        public string name;
        public byte[] rawData;
    }
}
