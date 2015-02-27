using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PlistToXmlConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("ERROR: No arguments. First argument should be path to .plist file. Second argument path to save xml. File will be overwritten");
                Console.ReadLine();
                return;
            }

            string uri = args[0];
            var fileInfo = new FileInfo(uri);
            if (!fileInfo.Exists)
            {
                Console.WriteLine("ERROR: No file with path \'{0}\'. First argument should be path to .plist file", uri);
                Console.ReadLine();
                return;
            }

            var doc = XDocument.Load(uri);
            var xml = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"), new XElement("Items"));
            var root = xml.Root;
            int level = 1;
            foreach (var dict in doc.Descendants("dict"))
            {
                var item = new XElement("Item", new XAttribute("id", level++));
                root.Add(item);
                XElement tempItem = null;
                foreach (var inner in dict.Elements())
                {
                    if (inner.Name == "key")
                    {
                        tempItem = new XElement(inner.Value);
                        item.Add(tempItem);
                    }
                    else if (tempItem != null)
                    {
                        tempItem.SetValue(inner.Value);
                        tempItem = null;
                    }
                }

                item.Add(new XElement("Rating", 0));
                item.Add(new XElement("MaxScore", 64));
                item.Add(new XElement("Score", 0));
                item.Add(new XElement("IsStarred", false));
                item.Add(new XElement("IsSolved", false));
                item.Add(new XElement("AttemptCount", 0));
            }

            var xmlPath = args[1];
            var xmlInfo = new FileInfo(xmlPath);
            if (xmlInfo.Exists)
                xmlInfo.Delete();

            xml.Save(xmlPath);

            Console.WriteLine("Done.");
            Console.ReadLine();
        }
    }
}
