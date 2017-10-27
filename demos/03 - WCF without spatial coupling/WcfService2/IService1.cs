using System.ServiceModel;

namespace WcfService2
{
	[ServiceContract]
	public interface IService1
	{
		[OperationContract(IsOneWay = true)]
		[ServiceKnownType("GetKnownTypes", typeof(MessageTypeFinder))]
		void Execute(IMessage message);
	}
}
