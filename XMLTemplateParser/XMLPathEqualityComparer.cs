using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace XMLTemplateParser
{
	class XmlPathEqualityComparer : IEqualityComparer<XElement>
	{
		public XmlPathEqualityComparer()
		{
			_ignoreAttributePredicate = (XAttribute) => false;
		}

		public XmlPathEqualityComparer(Func<XAttribute, bool> ignoreAttributePredicate)
		{
			_ignoreAttributePredicate = ignoreAttributePredicate;
		}

		public bool Equals(XElement x, XElement y)
		{
			if (x.Name != y.Name)
			{
				return false;
			}

			foreach (var attribute in y.Attributes())
			{
				if (_ignoreAttributePredicate != null && _ignoreAttributePredicate(attribute))
				{
					continue;
				}

				if (x.Attribute(attribute.Name)?.Value != attribute.Value)
				{
					return false;
				}
			}

			return true;
		}

		public int GetHashCode(XElement obj)
		{
			return GetHashCode();
		}

		private Func<XAttribute, bool> _ignoreAttributePredicate;
	}
}
