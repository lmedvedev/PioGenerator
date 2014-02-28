using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Schema;

namespace SPGeneratorForms
{
    [Serializable]
    public partial class Source
    {
        [XmlAttributeAttribute()]
        public string ServerName { get; set; }
        [XmlAttributeAttribute()]
        public string Database { get; set; }
        [XmlAttributeAttribute(DataType = "boolean")]
        public bool IsTrusterConnection { get; set; }
        [XmlAttributeAttribute()]
        public string UserLogin { get; set; }
        [XmlAttributeAttribute()]
        public string UserPassword { get; set; }

        public GenEntity DBClass { get; set; }
        public GenEntity DatClasses { get; set; }
        public GenEntity SetClasses { get; set; }
        public GenEntity ColumnsClasses { get; set; }
        public GenEntity FilterClasses { get; set; }
        public GenEntity Controllers { get; set; }
        public GenEntity FilterForms { get; set; }
        public GenEntity EntityForms { get; set; }

    }

    [Serializable]
    public partial class GenEntity
    {
        [XmlAttributeAttribute()]
        public string Path { get; set; }
        [XmlAttributeAttribute()]
        public bool Use { get; set; }
    }

    [Serializable]
    public partial class ServerList
    {
        public List<Source> Servers { get; set; }

        public static XmlConverter converter;
        static ServerList()
        {
            converter = new XmlConverter();
        }
        public ServerList()
        {
            Servers = new List<Source>();
        }

        public static ServerList Deserialize(XmlDocument XmlContent)
        {
            using (StringReader rd = new StringReader(XmlContent.OuterXml))
            {
                return new XmlSerializer(typeof(ServerList)).Deserialize(rd) as ServerList;
            }
        }

        public virtual XmlDocument Serialize()
        {
            return converter.Serialize(this.GetType(), this, false);
        }

        public static ServerList Load(string pathToXml)
        {
            return converter.Read<ServerList>(pathToXml);
        }

        public void Save(string pathToXml)
        {
            XmlDocument xd = this.Serialize();
            xd.Save(pathToXml);
        }

    }
    public class XmlConverter
    {
        private XmlSchemaSet Schemas = new XmlSchemaSet();

        public void AddSchema(string path)
        {
            using (TextReader stringReader = new StringReader(path))
            {
                XmlSchema sch = XmlSchema.Read(stringReader, null);
                Schemas.Add(sch);
            }
        }

        public void AddSchema(FileInfo file)
        {
            using (FileStream stream = new FileStream(file.FullName, FileMode.Open))
            {
                XmlSchema sch = XmlSchema.Read(stream, null);
                Schemas.Add(sch);
            }
        }

        public T Read<T>(string path)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.Schemas = Schemas;
            settings.ValidationEventHandler += ValidationE;
            settings.ValidationType = ValidationType.Schema;

            XmlSerializer ser = new XmlSerializer(typeof(T));
            ser.UnknownElement += new XmlElementEventHandler(ser_UnknownElement);
            ser.UnknownAttribute += new XmlAttributeEventHandler(ser_UnknownAttribute);
            ser.UnreferencedObject += new UnreferencedObjectEventHandler(ser_UnreferencedObject);

            using (XmlReader reader = XmlReader.Create(path, settings))
            {
                try
                {
                    object ags = ser.Deserialize(reader);
                    return (T)ags;
                }
                catch (Exception exp)
                {
                    string str = exp.Message;
                    throw exp;
                }
            }

        }

        public T ReadFromXML<T>(string xml)
        {
            return (T)Deserialize(typeof(T), xml);
        }

        public object Deserialize(Type type, string xml)
        {
            XmlSerializer sr = new XmlSerializer(type);
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.Schemas = Schemas;
            settings.ValidationEventHandler += ValidationE;
            settings.ValidationType = ValidationType.Schema;
            using (StringReader sreader = new StringReader(xml))
            {
                using (XmlReader reader = XmlReader.Create(sreader, settings))
                {
                    return sr.Deserialize(reader);
                }
            }
        }

        public object Deserialize(Type type, XmlDocument xmldoc)
        {
            return (xmldoc == null) ? null : Deserialize(type, xmldoc.OuterXml);
        }

        public XmlDocument ReadToDoc(string xml, bool Validate)
        {
            xml = xml.Trim();
            XmlDocument ret = new XmlDocument();
            //ret.Schemas = Schemas;
            ret.PreserveWhitespace = true;
            ret.LoadXml(xml);
            //if (ret.DocumentElement != null && ret.DocumentElement.Attributes["xml:space"] == null)
            //    ret.DocumentElement.Attributes.Append(ret.CreateAttribute("xml:space"));
            //ret.DocumentElement.Attributes["xml:space"].Value = "preserve";
            if (Validate && Schemas.Count > 0)
                ret.Validate(ValidationE);

            return ret;
        }
        public XmlDocument ReadToDoc(string xml)
        {
            return ReadToDoc(xml, true);
        }

        public XmlDocument ReadToDocFromFile(string path)
        {
            StreamReader reader = new StreamReader(path, true);
            XmlDocument ret = new XmlDocument();
            ret.Schemas = Schemas;
            ret.PreserveWhitespace = true;
            ret.LoadXml(reader.ReadToEnd());
            //if (ret.DocumentElement != null && ret.DocumentElement.Attributes["xml:space"] == null)
            //    ret.DocumentElement.Attributes.Append(ret.CreateAttribute("xml:space"));
            //ret.DocumentElement.Attributes["xml:space"].Value = "preserve";
            ret.Validate(ValidationE);
            return ret;
        }

        //UTF8
        public static Encoding utf8 = new UTF8Encoding();
        public static Encoding windows1251 = Encoding.GetEncoding(1251);

        public XmlDocument Serialize(Type type, object val)
        {
            return Serialize(type, val, true);
        }
        public XmlDocument Serialize(Type type, object val, bool validate, Encoding enc)
        {
            //XmlDocument ret = new XmlDocument();

            XmlSerializer sr = new XmlSerializer(type);

            //if (validate)
            //    ret.Schemas = Schemas;

            MemoryStream stream = new MemoryStream();
            using (XmlTextWriter tw = new XmlTextWriter(stream, enc))
            {
                try
                {
                    sr.Serialize(tw, val);
                }
                catch (Exception exp)
                {
                    string str = exp.Message;
                    throw;
                }
            }
            return ReadToDoc(enc.GetString(stream.ToArray()), validate);
        }
        public XmlDocument Serialize(Type type, object val, bool validate)
        {
            return Serialize(type, val, validate, utf8);
        }

        public string Validate(string xml)
        {
            string ret = "";
            if (Schemas.Count == 0) return ret;
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            doc.Schemas = Schemas;
            try
            {
                doc.Validate(ValidationE);
                return "";
            }
            catch (Exception exp)
            {
                ret = exp.Message;
            }
            return ret;
        }

        private void ValidationE(Object sender, ValidationEventArgs e)
        {
            throw e.Exception;
            //throw new Exception(string.Format("{0} \nв файле {1} \nLine number: {2} \nLine position: {3}", e.Message,e.Exception.SourceUri,e.Exception.LineNumber,e.Exception.LinePosition));
        }
        void ser_UnreferencedObject(object sender, UnreferencedObjectEventArgs e)
        {
            throw new Exception("UnreferencedObject: " + e.ToString());
        }

        void ser_UnknownNode(object sender, XmlNodeEventArgs e)
        {
            throw new Exception("UnknownNode: " + e.ToString());
        }

        void ser_UnknownAttribute(object sender, XmlAttributeEventArgs e)
        {
            throw new Exception("UnknownAttribute: " + e.ToString());
        }

        void ser_UnknownElement(object sender, XmlElementEventArgs e)
        {
            throw new Exception("UnknownElement: " + e.ToString());
        }
    }
}
