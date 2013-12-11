using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace MinutesToMidnight
{
    class ObjLoader<T>
    {

        private DataContractSerializer superserial;

        public ObjLoader(){
            superserial = new DataContractSerializer(typeof(T));
        }

        public List<T> Load(string filename)
        {
            List<T> value = new List<T>();
            if (String.IsNullOrEmpty(filename))
            {
                return null;
            }
            FileStream fs = new FileStream(filename, FileMode.Open);
            XmlDictionaryReader reader =
                XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas());

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (superserial.IsStartObject(reader))
                        {
                            value.Add((T)superserial.ReadObject(reader));
                        }
                        break;
                }
            }

            fs.Close();


            /*
            FileStream file = File.OpenWrite("testout.xml");

            superserial.WriteObject(file, testTruth);

            file.Close();
             */

            return value;
        }

        public void Save(string filename, T obj)
        {

            FileStream file = File.OpenWrite(filename);

            superserial.WriteObject(file, obj);

            file.Close();
        }
    }


}