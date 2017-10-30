using System.ServiceModel;

namespace WcfService
{
	[ServiceContract]
	public interface IService
	{
		[OperationContract(IsOneWay = true)]
		[ServiceKnownType("GetKnownTypes", typeof(MessageTypeFinder))]
		void Execute(IMessage message);
	}
}
