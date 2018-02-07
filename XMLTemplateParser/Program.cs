using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace XMLTemplateParser
{
	class Program
	{
		static void Main(string[] args)
		{
			XDocument xDoc = XDocument.Load("template_test.xml");
			var parserBuilder = new ParserBuilder();
			Dictionary<string, XAttribute> map = parserBuilder.CreateParserMap(xDoc.Root);

			var entity = parserBuilder.ParseWithMap<TestClass>(XDocument.Load("doc_test.xml"), map);
		}
	}

	internal class TestClass
	{
		public string Type { get; set; }
		public TestInternalClass Object { get; set; }
	}

	internal class TestInternalClass
	{
		public string Version { get; set; }
		public string Status { get; set; }
		public string CompetitionCode { get; set; }
		public string DocumentCode { get; set; }
	}

	public class ParserBuilder
	{
		public Dictionary<string, XAttribute> CreateParserMap(XElement templateRoot)
		{
			Dictionary<string, XAttribute> propertiesMap = new Dictionary<string, XAttribute>();

			foreach (var attribute in templateRoot.Attributes())
			{
				if (HasPropertyMarker(attribute))
				{
					propertiesMap.Add(attribute.Value.Remove(0, 1), attribute);
				}
			}

			foreach (var element in templateRoot.Elements())
			{
				var childProps = CreateParserMap(element);

				foreach (var prop in childProps)
				{
					propertiesMap.Add(prop.Key, prop.Value);
				}
			}

			return propertiesMap;
		}

		public TEntity ParseWithMap<TEntity>(XDocument document, Dictionary<string, XAttribute> map)
		{
			TEntity entity = Activator.CreateInstance<TEntity>();

			foreach (var property in map)
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
			var xElementComparer = new XMLPathEqualityComparer((a) => HasPropertyMarker(a));

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

		private bool HasPropertyMarker(XAttribute attribute)
		{
			Regex propertyNameRegex = new Regex("^@\\w+");

			return propertyNameRegex.IsMatch(attribute.Value);
		}

		private void SetPropertyByPath(object entity, string path, object value)
		{
			if (path.Contains('.'))
			{
				var propertyComponents = path.Split('.');
				var nextPropertyName = propertyComponents[0];

				if (nextPropertyName.Contains(":"))
				{
					return;
				}

				var nextProperty = entity.GetType()
					.GetProperty(propertyComponents[0]);

				if (nextProperty.GetValue(entity) == null)
				{
					nextProperty
						.SetValue(entity, Activator.CreateInstance(nextProperty.PropertyType));
				}

				SetPropertyByPath(nextProperty.GetValue(entity), string.Join('.', propertyComponents.Skip(1)), value);
			}
			else
			{
				if (path.Contains(":"))
				{
					return;
				}

				entity.GetType()
					.GetProperty(path)
					.SetValue(entity, value);
			}
		}
	}
}
