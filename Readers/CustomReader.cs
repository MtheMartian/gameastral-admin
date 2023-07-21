using System.Xml;

namespace GameStarBackend.Readers
{
    public class CustomReader
    {
        static public Dictionary<string, string[]> ReadMyXml(string uri)
        {
            Dictionary<string, string[]> keyValues = new()
            {
                ["keyInfo"] = new string[4],
                ["descriptor"] = new string[6],
            };

            XmlReader textReader = XmlReader.Create($@"{uri}");

            int count = 0;
            int resultsCount = 0;
            int resultsCount2 = 1;
            textReader.Read();

            while (textReader.Read())
            {
                textReader.MoveToElement();

                if (textReader.LocalName == "key")
                {
                    if (textReader.HasAttributes)
                    {
                        for (int i = 1; i < textReader.AttributeCount; i++)
                        {
                            keyValues["keyInfo"][0] = textReader.GetAttribute(i - 1);
                        }
                    }
                }

                if (textReader.NodeType == XmlNodeType.Text && count != 5)
                {
                    keyValues["keyInfo"][resultsCount2] = textReader.Value;
                    resultsCount2++;
                }

                if (textReader.LocalName == "descriptor" || textReader.LocalName == "encryption"
                    || textReader.LocalName == "validation" || textReader.LocalName == "masterKey")
                {
                    if (textReader.HasAttributes)
                    {
                        for (int i = 0; i < textReader.AttributeCount; i++)
                        {
                            keyValues["descriptor"][resultsCount] = textReader.GetAttribute(i);
                            resultsCount++;
                        }

                    }
                    count++;
                }
                if (textReader.NodeType == XmlNodeType.Comment)
                {
                    textReader.Skip();
                }
                if (textReader.NodeType == XmlNodeType.Text && count == 5)
                {
                    keyValues["descriptor"][5] = textReader.Value;
                }
            }
            textReader.Close();
            Console.WriteLine("File successfully created!");
            return keyValues;
        }
    }
}
