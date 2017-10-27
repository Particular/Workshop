using System;
using System.Linq;
using System.Reflection;
using System.Diagnostics;

namespace WcfService2
{
	public class Service1 : IService1
	{
		public void Execute(IMessage message)
		{
			Assembly assembly = (message.GetType()).Assembly;

            // Get all classes that implement IHandleMessages<>
			var allMessageHandlers =
					from type in assembly.GetTypes()
					where !type.IsAbstract
					from interfaceType in type.GetInterfaces()
					where interfaceType.IsGenericType
					where interfaceType.GetGenericTypeDefinition() == typeof(IHandleMessages<>)
					select type;

            // Filter all classes that implement this specific message type
			Type messageInterface = typeof(IHandleMessages<>).MakeGenericType(message.GetType());
			var myMessageHandlers = allMessageHandlers
											.Where(type => type.GetInterfaces()
											.Any(it => it == messageInterface))
											.Distinct();

            // Loop through all handlers found
			foreach (var handler in myMessageHandlers)
			{
				object handlerInstance = Activator.CreateInstance(handler);

                // Find methods that are called "Handle" and expect as parameter this message type in particular
				var methods = from m in handler.GetMethods()
											where m.Name == "Handle"
											from p in m.GetParameters()
											where p.ParameterType == message.GetType()
											select m;

                // Invoke the `Handle` method with the parameter
				var methodInfo = methods.Single();
				methodInfo.Invoke(handlerInstance, new[] { message });
			}
		}
	}
}
