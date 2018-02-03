using System.Threading;
using System.Threading.Tasks;

namespace Playground.Web.Services
{
    public interface IEchoService
    {
        Task<string> Echo(string message,
            CancellationToken cancellationToken = default(CancellationToken)
            );
    }
}