namespace Story.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IStory
    {
        string Name { get; }

        string InstanceId { get; }

        void Start();

        void Stop(Task task);

        TimeSpan Elapsed { get; }

        long ElapsedTicks { get; }

        long ElapsedMilliseconds { get; }

        IStoryData Data { get; }

        IStoryLog Log { get; }

        IStory Parent { get; }
    }
}
