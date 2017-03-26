using Oni2Xml.Readers;
using Oni2Xml.TypeData;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Oni2Xml.SaveData
{
    static class GameObjectDataParser
    {


        public static GameObjectData Parse(IReader reader, TypeReader typeReader)
        {

            var data = new GameObjectData()
            {
                roots = new List<GameObjectRoot>()
            };

            int numPrefabs = reader.ReadInt32();
            for (var i = 0; i< numPrefabs; i++)
            {
                var root = ParseRoot(reader, typeReader);
                data.roots.Add(root);
            }

            return data;
        }


        private static GameObjectRoot ParseRoot(IReader reader, TypeReader typeReader)
        {
            string name = reader.ReadKleiString();
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
                var gameObj = ParseGameObject(reader, typeReader);
                root.GameObject.Add(gameObj);
            }


            if (reader.Position - startPos != length)
            {
                Debug.WriteLine(string.Format("WARN: Prefab {0} read differing bytes than length", name));
                reader.SkipBytes(length - (reader.Position - startPos));
            }

            return root;
        }


        private static GameObject ParseGameObject(IReader reader, TypeReader typeReader)
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
                var component = ParseComponent(reader, typeReader);
                componentLength = reader.Position - componentLength;
                totalComponentLength += componentLength;
                gameObj.Components.Add(component);
            }

            return gameObj;
        }

        private static Component ParseComponent(IReader reader, TypeReader typeReader)
        {
            string key = reader.ReadKleiString();
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

            if (typeReader.HasTemplate(key))
            {
                var templateReader = new FastReader(data);
                component.parsedData = typeReader.ReadTemplateObject(key, templateReader);
                if (!templateReader.IsFinished)
                {
                    component.saveLoadableDetailsData = templateReader.ReadBytes(data.Length - templateReader.Position);
                    Debug.WriteLine(string.Format("WARN: Component {0} template did not read all data.  This may be a sign it uses additional parsing (ISaveLoadableDetailJson).", key));
                }
            }
            else
            {
                Debug.WriteLine(string.Format("WARN: Component {0} has no matching type template", key));
            }

            // TODO: Identify mono behaviors from key and try to make sense of their data.

            return component;
        }
    }

    struct GameObjectData
    {
        public IList<GameObjectRoot> roots;
    }

    [DebuggerDisplay("GameObectRoot {tag}")]
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

    [DebuggerDisplay("Component {name}")]
    struct Component
    {
        public string name;
        public byte[] rawData;
        public ObjectTemplateData parsedData;
        public byte[] saveLoadableDetailsData;
    }
}
