using System.Threading;
using System.Threading.Tasks;

namespace ALS.Services.Abstractions
{
    public interface IMessageService
    {
        Task Start(CancellationToken cancellationToken);
    }
}
