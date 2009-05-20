using System.ServiceModel;

namespace Sneal.ProxyConsole.WcfService
{
    [ServiceContract(
        Name = "ConsoleListener",
        Namespace = "http://proxyconsole.sneal.net")]
    public interface IConsoleListener
    {
        [OperationContract(IsOneWay = true)]
        void ExecutionComplete(ExecutionFinishedMessage completeMessage);

        [OperationContract(IsOneWay = true)]
        void ExecutionProgress(ExecutionProgressMessage progressMessage);
    }
}
