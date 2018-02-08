using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace XMLTemplateParser.Configuration
{
	public class XmlTemplateMappingConfiguration
	{
		public XmlTemplateMappingConfiguration(string propertyNameMarker, string listPropertyMarker)
		{
			PropertyNameMarker = propertyNameMarker;
			ListPropertyMarker = listPropertyMarker;
		}

		public string PropertyNameMarker
		{
			get
			{
				return _propertyNameMarker;
			}
			set
			{
				_propertyNameMarkerRegex = new Regex($"^{value}\\w+");
				_propertyNameMarker = value;
			}
		}

		public string ListPropertyMarker
		{
			get
			{
				return _listPropertyMarker;
			}
			set
			{
				_listPropertyMarkerRegex = new Regex($"\\w+{value}$");
				_listPropertyMarker = value;
			}
		}

		public bool HasPropertyMarker(XAttribute attribute)
		{
			return _propertyNameMarkerRegex.IsMatch(attribute.Value);
		}

		public bool IsListProperty(string propertyName)
		{
			return _listPropertyMarkerRegex.IsMatch(propertyName);
		}

		private Regex _propertyNameMarkerRegex;
		private string _propertyNameMarker;
		private Regex _listPropertyMarkerRegex;
		private string _listPropertyMarker;
	}
}
