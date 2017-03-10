using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Oni2Xml.SaveData
{
    static class GameObjectDataParser
    {


        public static GameObjectData Parse(IReader reader)
        {

            var data = new GameObjectData()
            {
                roots = new List<GameObjectRoot>()
            };

            int numPrefabs = reader.ReadInt32();
            for (var i = 0; i< numPrefabs; i++)
            {
                var root = ParseRoot(reader);
                data.roots.Add(root);
            }

            return data;
        }


        private static GameObjectRoot ParseRoot(IReader reader)
        {
            string name = reader.ReadKleiString();
            Debug.WriteLine("Parsing tppy " + name);
            int capacity = reader.ReadInt32();
            int length = reader.ReadInt32();

            int startPos = reader.Position;

            var root = new GameObjectRoot()
            {
                tag = name,
                capacity = capacity,
                length = length,
                GameObject = new List<GameObject>()
            };

            for(var i = 0; i < capacity; i++)
            {
                var gameObj = ParseGameObject(reader);
                root.GameObject.Add(gameObj);
            }


            if (reader.Position - startPos != length)
            {
                Debug.WriteLine(string.Format("WARN: Prefab {0} read differing bytes than length", name));
                reader.SkipBytes(length - (reader.Position - startPos));
            }

            return root;
        }


        private static GameObject ParseGameObject(IReader reader)
        {
            var gameObj = new GameObject();

            gameObj.Components = new List<Component>();
            gameObj.position = reader.ReadVector3();
            gameObj.rotation = reader.ReadQuaternion();
            gameObj.scale = reader.ReadVector3();

            gameObj.folder = reader.ReadByte();


            var numComponents = reader.ReadInt32();
            var totalComponentLength = 0;
            for (var i = 0; i < numComponents; i++)
            {
                int componentLength = reader.Position;
                var component = ParseComponent(reader);
                componentLength = reader.Position - componentLength;
                totalComponentLength += componentLength;
                gameObj.Components.Add(component);
            }

            return gameObj;
        }

        private static Component ParseComponent(IReader reader)
        {
            string key = reader.ReadKleiString();
            Debug.WriteLine("Parsing component " + key);
            int length = reader.ReadInt32();

            int startPos = reader.Position;

            byte[] data = reader.ReadBytes(length);

            var component = new Component()
            {
                name = key,
                rawData = data
            };


            if (reader.Position - startPos != length)
            {
                Debug.WriteLine(string.Format("WARN: Component {0} read differing bytes than length", key));
                reader.SkipBytes(length - (reader.Position - startPos));
            }

            // TODO: Identify mono behaviors from key and try to make sense of their data.

            return component;
        }
    }

    struct GameObjectData
    {
        public IList<GameObjectRoot> roots;
    }

    struct GameObjectRoot
    {
        public string tag;
        public int capacity;
        public int length;

        public IList<GameObject> GameObject;
    }

    class GameObject
    {
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
