using System.ServiceModel;

namespace Sneal.ProxyConsole.WcfService
{
    [ServiceContract(
        Name="ConsoleRunner",
        Namespace="http://proxyconsole.sneal.net",
        CallbackContract=typeof(IConsoleListener),
        SessionMode=SessionMode.Required)]
    public interface IConsoleRunner
    {
        [OperationContract]
        void Run(StartExecutionMessage requestMsg);
    }
}
