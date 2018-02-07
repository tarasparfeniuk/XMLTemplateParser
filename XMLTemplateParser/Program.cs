using System.Collections.Generic;
using System.Xml.Linq;

namespace XMLTemplateParser
{
	class Program
	{
		static void Main(string[] args)
		{
			var mappingConfig = new XmlTemplateMappingConfiguration("@", ":");
			var xDoc = XDocument.Load("template_test.xml");
			var templateMapBuilder = new TemplateMapBuilder(mappingConfig);

			Dictionary<string, XAttribute> map = templateMapBuilder.CreateTemplateMap(xDoc.Root);

			var xmlParser = new XmlTemplateParser(map, mappingConfig);

			var entity = xmlParser.ParseDocument(XDocument.Load("doc_test.xml"));
		}
	}
}
