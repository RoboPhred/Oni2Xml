﻿namespace Oni2Xml.TypeData
{
    public enum SerializationTypeInfo : byte
    {
        UserDefined = 0,
        SByte = 1,
        Byte = 2,
        Boolean = 3,
        Int16 = 4,
        UInt16 = 5,
        Int32 = 6,
        UInt32 = 7,
        Int64 = 8,
        UInt64 = 9,
        Single = 10,
        Double = 11,
        String = 12,
        Enumeration = 13,
        Vector2I = 14,
        Vector2 = 15,
        Vector3 = 16,
        Array = 17,
        Pair = 18,
        Dictionary = 19,
        List = 20,
        HashSet = 21,
        Colour = 22,
        VALUE_MASK = 127,
        IS_GENERIC_TYPE = 128,
    }
}
