using System;
using System.Collections;
using System.Collections.Generic;

namespace XMLTemplateParser.Mappers
{
	public class DictionaryToEntityMapper
	{
		public bool TryMap<TEntity>(Dictionary<string, object> properties, out TEntity entity) where TEntity : class
		{
			try
			{
				entity = Map<TEntity>(properties);
				return true;
			}
			catch(Exception ex)
			{
				entity = null;
				return false;
			}
		}

		public object Map(Type entityType, Dictionary<string, object> properties)
		{
			object entity = Activator.CreateInstance(entityType);

			foreach (var property in properties)
			{
				var entityProp = entity
					.GetType()
					.GetProperty(property.Key);

				if (entityProp == null)
				{
					throw new InvalidCastException($"Can\'t map the dictionary to {entity.GetType().FullName}. Property {property} doesn\'t exists in type {entity.GetType().FullName}.");
				}

				if (property.Value is Dictionary<string, object>)
				{
					entityProp.SetValue(entity, Map(entityProp.PropertyType, property.Value as Dictionary<string, object>));
					continue;
				}

				if (property.Value is IEnumerable<Dictionary<string, object>>)
				{
					if (entityProp.PropertyType.GetInterface(typeof(IEnumerable).Name) == null)
					{
						throw new InvalidCastException($"Can\'t map the dictionary to {entity.GetType().FullName}. Properties with the same name({property}) have different types.");
					}

					var genericArgument = entityProp.PropertyType.GetGenericArguments()[0];

					entityProp.SetValue(entity, Activator.CreateInstance(entityProp.PropertyType));

					var listType = typeof(List<>).MakeGenericType(new Type[] { genericArgument });
					IList list = (IList)Activator.CreateInstance(listType);

					foreach (var item in property.Value as IEnumerable<Dictionary<string, object>>)
					{
						list.Add(Convert.ChangeType(Map(genericArgument, item), genericArgument));
					}

					entityProp.SetValue(entity, list);
					continue;
				}



			}

			return entity;
		}

		public TEntity Map<TEntity>(Dictionary<string, object> properties) where TEntity : class
		{
			return Map(typeof(TEntity), properties) as TEntity;
		}
	}
}
