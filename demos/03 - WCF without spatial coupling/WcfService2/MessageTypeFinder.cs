using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace WcfService2
{
	public class MessageTypeFinder
	{
		public static IEnumerable<Type> GetKnownTypes(ICustomAttributeProvider provider)
		{
			IEnumerable<Type> query =
					from type in typeof(IMessage).Assembly.GetTypes()
					where typeof(IMessage).IsAssignableFrom(type)
					select type;

			return query.ToArray();
		}
	}
}