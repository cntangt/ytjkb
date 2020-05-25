using System.Collections.Generic;

namespace FytSoa.Service.DtoModel.Cms
{
    public class IndexModel
    {
        public IEnumerable<PlatformInfo> PlatformInfos { get; set; }

        public IEnumerable<TrendInfo> Trends { get; set; }
    }
}
