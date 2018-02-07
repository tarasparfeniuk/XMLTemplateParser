using System.Collections.Generic;
using System.Xml.Linq;

namespace XMLTemplateParser
{
	public class TemplateMapBuilder
	{
		public TemplateMapBuilder(XmlTemplateMappingConfiguration config)
		{
			_mappingConfig = config;
		}

		public Dictionary<string, XAttribute> CreateTemplateMap(XElement templateRoot)
		{
			Dictionary<string, XAttribute> propertiesMap = new Dictionary<string, XAttribute>();

			foreach (var attribute in templateRoot.Attributes())
			{
				if (_mappingConfig.HasPropertyMarker(attribute))
				{
					propertiesMap.Add(attribute.Value.Remove(0, 1), attribute);
				}
			}

			foreach (var element in templateRoot.Elements())
			{
				var childProps = CreateTemplateMap(element);

				foreach (var prop in childProps)
				{
					propertiesMap.Add(prop.Key, prop.Value);
				}
			}

			return propertiesMap;
		}

		private readonly XmlTemplateMappingConfiguration _mappingConfig;
	}
}
