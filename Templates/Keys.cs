using GameStarBackend.Models;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.Xml;

namespace GameStarBackend.Templates
{
    public class Keys
    {
        static public void WriteXMLKeyFile(EncKeys keyDB)
        {
            var tempFolder = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "GS-Keys"));

            XmlWriterSettings settings = new XmlWriterSettings()
            {
                Indent = true,
            };
            XmlWriter textWriter = XmlWriter.Create(
                $@"{Path.Combine(tempFolder.FullName, $"key-{keyDB.KeyId}.xml")}", settings);

             textWriter.WriteStartDocument();

            //--- Key ---//
            textWriter.WriteStartElement("key");
            textWriter.WriteAttributeString("id", keyDB.KeyId);
            textWriter.WriteAttributeString("version", "1");

            textWriter.WriteElementString("creationDate", keyDB.CreationDate);
            textWriter.WriteElementString("activationDate", keyDB.ActDate);
            textWriter.WriteElementString("expirationDate", keyDB.ExpDate);
            //--- Descriptor ---//
            textWriter.WriteStartElement("descriptor");
            textWriter.WriteAttributeString("deserializerType", keyDB.Descriptor[0]);
            textWriter.WriteStartElement("descriptor");

            textWriter.WriteStartElement("encryption");
            textWriter.WriteAttributeString("algorithm", keyDB.Descriptor[1]);
            textWriter.WriteEndElement();

            textWriter.WriteStartElement("validation");
            textWriter.WriteAttributeString("algorithm", keyDB.Descriptor[2]);
            textWriter.WriteEndElement();

            textWriter.WriteStartElement("masterKey");
            textWriter.WriteStartAttribute("p4", "requiresEncryption", keyDB.Descriptor[4]);
            textWriter.WriteValue(true);
            textWriter.WriteEndAttribute();
            textWriter.WriteAttributeString("xmlns", "p4", null, keyDB.Descriptor[4]);
            textWriter.WriteComment(" Warning: the key below is in an unencrypted form. ");
            textWriter.WriteElementString("value", keyDB.Descriptor[5]);
            //--- EncKey End ---//
            textWriter.WriteEndElement();
            //--- masterKey End ---//
            textWriter.WriteEndElement();
            //--- Descriptor End ---//
            textWriter.WriteEndElement();

            //--- End Key ---//
            textWriter.WriteEndElement();

            textWriter.Close();
        }
    }
}
