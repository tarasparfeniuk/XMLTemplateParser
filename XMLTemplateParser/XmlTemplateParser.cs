using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace XMLTemplateParser
{
	public class XmlTemplateParser
	{
		public XmlTemplateParser(Dictionary<string, XAttribute> templateMap, XmlTemplateMappingConfiguration config)
		{
			_templateMap = templateMap;
			_mappingConfig = config;
		}

		public Dictionary<string, object> ParseDocument(XDocument document)
		{
			Dictionary<string, object> entity = new Dictionary<string, object>();

			foreach (var property in _templateMap)
			{
				string value = null;

				if (TryGetValueByAttribute(document.Root, property.Value, out value))
				{
					SetPropertyByPath(entity, property.Key, value);
				}
			}

			return entity;
		}

		private bool TryGetValueByAttribute(XElement root, XAttribute attribute, out string value)
		{
			var xElementComparer = new XmlPathEqualityComparer((a) => _mappingConfig.HasPropertyMarker(a));

			if (xElementComparer.Equals(root, attribute.Parent))
			{
				foreach (var targetAttribute in root.Attributes())
				{
					if (targetAttribute.Name == attribute.Name)
					{
						value = targetAttribute.Value;

						return value != null;
					}
				}
			}

			foreach (var child in root.Elements())
			{
				if (TryGetValueByAttribute(child, attribute, out value))
				{
					return true;
				}
			}

			value = null;

			return false;
		}

		private void SetPropertyByPath(Dictionary<string, object> entity, string path, object value)
		{
			if (path.Contains('.'))
			{
				var propertyComponents = path.Split('.');
				var nextPropertyName = propertyComponents[0];

				if (_mappingConfig.IsListProperty(nextPropertyName))
				{
					nextPropertyName = nextPropertyName.Substring(0, nextPropertyName.IndexOf(_mappingConfig.ListPropertyMarker));

					if (!entity.ContainsKey(nextPropertyName))
					{
						entity.Add(nextPropertyName, new List<Dictionary<string, object>>());
					}

					var listItem = new Dictionary<string, object>();

					SetPropertyByPath(
						listItem,
						string.Join('.', propertyComponents.Skip(1)),
						value);

					(entity[nextPropertyName] as List<Dictionary<string, object>>).Add(listItem);

					return;
				}

				if (!entity.ContainsKey(nextPropertyName))
				{
					entity.Add(nextPropertyName, new Dictionary<string, object>());
				}

				SetPropertyByPath(
					entity[nextPropertyName] as Dictionary<string, object>,
					string.Join('.', propertyComponents.Skip(1)),
					value);
			}
			else
			{
				if (!entity.ContainsKey(path))
				{
					entity.Add(path, value);
				}
			}
		}	

		private readonly Dictionary<string, XAttribute> _templateMap;
		private readonly XmlTemplateMappingConfiguration _mappingConfig;
	}
}
