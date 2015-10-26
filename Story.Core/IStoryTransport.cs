using Story.Core.Handlers;
using System.Collections.Generic;
using System.Threading;

namespace Story.Core
{
    using System.Threading.Tasks;

    public interface IStoryTransport
    {
        Task SendAsync(IEnumerable<IStory> stories);

        Task<IStory> GetNextAsync(CancellationToken token);
    }
}
