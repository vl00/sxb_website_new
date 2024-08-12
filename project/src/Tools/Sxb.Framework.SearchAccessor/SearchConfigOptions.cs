using Microsoft.Extensions.Options;

namespace Sxb.Framework.SearchAccessor
{
    public class SearchConfigOptions
    {
        public SearchConfigOptions(IOptionsSnapshot<SearchConfig> options)
        {
            ConnectionConfig = options.Value;
        }

        public SearchConfig ConnectionConfig { get; }
    }
}
