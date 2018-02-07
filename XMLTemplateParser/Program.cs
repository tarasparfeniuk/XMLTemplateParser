using System.Collections.Generic;
using System.Xml.Linq;

namespace XMLTemplateParser
{
	class Program
	{
		static void Main(string[] args)
		{
			// ------------------------------------------------------------------------------------ //

			var mappingConfig = new XmlTemplateMappingConfiguration("@", ":");
			var xDoc = XDocument.Load("template_test.xml");
			var templateMapBuilder = new TemplateMapBuilder(mappingConfig);

			Dictionary<string, XAttribute> map = templateMapBuilder.CreateTemplateMap(xDoc.Root);

			var xmlParser = new XmlTemplateParser(map, mappingConfig);

			var entity = xmlParser.ParseDocument(XDocument.Load("doc_test.xml"));

			// ------------------------------------------------------------------------------------ //

			var internalEntity = new Dictionary<string, object>();
			internalEntity.Add("Value", "test_internal");

			var internalList = new List<Dictionary<string, object>>();
			internalList.Add(internalEntity);
			internalList.Add(internalEntity);

			var testEntity = new Dictionary<string, object>();

			testEntity.Add("Name", "test");
			testEntity.Add("Entity", internalEntity);
			testEntity.Add("Entities", internalList);

			var mapper = new DictionaryToEntityMapper();

			var outEntity = mapper.Map<TestEntity>(testEntity);

			// ------------------------------------------------------------------------------------ //
		}
	}

	class TestEntity
	{
		public string Name { get; set; }

		public TestInternalEntity Entity { get; set; }

		public List<TestInternalEntity> Entities { get; set; }
	}

	class TestInternalEntity
	{
		public string Value { get; set; }

	}
}
