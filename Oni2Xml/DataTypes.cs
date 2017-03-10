using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oni2Xml
{
    static class DataReaderExtensions
    {
        public static Vector2I ReadVector2I(this IReader reader)
        {
            Vector2I vector2I;
            vector2I.x = reader.ReadInt32();
            vector2I.y = reader.ReadInt32();
            return vector2I;
        }

        public static Vector2 ReadVector2(this IReader reader)
        {
            Vector2 vector2;
            vector2.x = reader.ReadSingle();
            vector2.y = reader.ReadSingle();
            return vector2;
        }

        public static Vector3 ReadVector3(this IReader reader)
        {
            Vector3 vector3;
            vector3.x = reader.ReadSingle();
            vector3.y = reader.ReadSingle();
            vector3.z = reader.ReadSingle();
            return vector3;
        }

        public static Color ReadColour(this IReader reader)
        {
            byte num1 = reader.ReadByte();
            byte num2 = reader.ReadByte();
            byte num3 = reader.ReadByte();
            byte num4 = reader.ReadByte();
            Color color;
            color.r = (float)num1 / (float)byte.MaxValue;
            color.g = (float)num2 / (float)byte.MaxValue;
            color.b = (float)num3 / (float)byte.MaxValue;
            color.a = (float)num4 / (float)byte.MaxValue;
            return color;
        }

        public static Quaternion ReadQuaternion(this IReader reader)
        {
            return new Quaternion()
            {
                x = reader.ReadSingle(),
                y = reader.ReadSingle(),
                z = reader.ReadSingle(),
                w = reader.ReadSingle()
            };
        }
    }

    struct Vector2I
    {
        public int x;
        public int y;
    }

    struct Vector2
    {
        public float x;
        public float y;
    }

    struct Vector3
    {
        public float x;
        public float y;
        public float z;
    }

    struct Color
    {
        public float r;
        public float g;
        public float b;
        public float a;
    }

    struct Quaternion
    {
        public float x;
        public float y;
        public float z;
        public float w;
    }
}
