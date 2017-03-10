using Oni2Xml.TypeData;
using System;
using System.Collections.Generic;

namespace Oni2Xml.SaveData
{
    class SaveData
    {
        private static readonly char[] SAVE_HEADER = new char[4]
{
            'K',
            'S',
            'A',
            'V'
};


        public static SaveData Parse(IReader reader)
        {
            var data = new SaveData();


            // Header
            data.header = HeaderParser.Parse(reader);



            // Templates
            var numDeserializationTemplates = reader.ReadInt32();
            data.templates = new List<TypeTemplate>();
            for (var i = 0; i < numDeserializationTemplates; i++)
            {
                var template = TypeTemplate.Parse(reader);
                data.templates.Add(template);
            }



            // "world" string
            var worldHeader = reader.ReadKleiString();
            if (worldHeader != "world")
            {
                throw new Exception("Invalid world header.  Expected header string to be \"world\", but got " + worldHeader.Substring(0, 100));
            }





            // sections
            var typeReader = new TypeReader(data.templates);
            data.sections = new Dictionary<string, ObjectTemplateData>();
            //while(!reader.IsFinished)
            //{
            //    var sectionName = reader.ReadKleiString();
            //    var sectionData = typeReader.ReadTemplateObject(sectionName, reader);
            //    data.sections.Add(sectionName, sectionData);
            //}
            /*
             * Load:
             *  SaveFileRoot (Klei.SaveFileRoot ?)
             *  Game.Settings (Game+Settings ?)
             *  >> SaveManager.Load <<
             *  Game.GameSaveData (Game+GameSaveData ?)
             */


            LoadSection("Klei.SaveFileRoot", data, typeReader, reader);
            LoadSection("Game+Settings", data, typeReader, reader);


            char[] chArray = reader.ReadChars(SAVE_HEADER.Length);
            if (chArray == null || chArray.Length != SAVE_HEADER.Length)
                throw new Exception("ManagerHeader length mismatch");
            for (int index = 0; index < SAVE_HEADER.Length; ++index)
            {
                if ((int)chArray[index] != (int)SAVE_HEADER[index])
                    throw new Exception("ManagerHeader content mismatch");
            }
            int verMajor = reader.ReadInt32();
            int verMinor = reader.ReadInt32();
            if (verMajor != 6 || verMinor > 0)
            {
                throw new Exception(string.Format("SAVE FILE VERSION MISMATCH! Expected {0}.{1} but got {2}.{3}", 6, 0, verMajor, verMinor));
            }


            data.prefabData = PrefabParser.Parse(reader);

            LoadSection("Game+GameSaveData", data, typeReader, reader);

            return data;
        }

        private static void LoadSection(string sectionName, SaveData data, TypeReader typeReader, IReader reader)
        {
            var rootName = reader.ReadKleiString();
            if (rootName != sectionName)
            {
                throw new Exception(string.Format("Expected data object named {0} but got {1}", sectionName, rootName.Substring(1000)));
            }
            data.sections.Add(
                sectionName,
                typeReader.ReadTemplateObject(sectionName, reader)
            );
        }


        Header header;
        IList<TypeTemplate> templates;
        PrefabData prefabData;
        IDictionary<string, ObjectTemplateData> sections;
    }
}
