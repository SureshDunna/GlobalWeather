using System.Web;

namespace GlobalWeatherApi.BusinessLogic
{
    public interface IExecutionContext
    {
        string BinDirectory { get; }
    }

    public class ExecutionContext : IExecutionContext
    {
        public string BinDirectory => HttpRuntime.BinDirectory;
    }
}