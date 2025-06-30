using System.Threading;
using System.Threading.Tasks;

namespace Blazor.RCL.Infrastructure.BackgroundServices.Interfaces
{
    /// <summary>
    /// Defines the contract for services that poll for request status
    /// </summary>
    public interface IRequestPollingService
    {
        /// <summary>
        /// Polls for pending requests and processes them
        /// </summary>
        /// <param name="checkExternalDatabase">Whether to check the external database for request status</param>
        /// <param name="cancellationToken">Cancellation token to stop the polling</param>
        Task PollAndProcessRequestsAsync(bool checkExternalDatabase, CancellationToken cancellationToken);
    }
}
