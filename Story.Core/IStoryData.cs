namespace Story.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IStoryData : IEnumerable<KeyValuePair<string, object>>
    {
        object this[string key] { set; }
    }
}
