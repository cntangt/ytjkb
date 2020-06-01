using System;

namespace FytSoa.Service.DtoModel.Cms
{
    public class TrendInfo
    {
        public DateTime Day { get; set; }
        public long CountTrade { get; set; }
        public long TotalTrade { get; set; }
        public long CountRefund { get; set; }
        public long TotalRefund { get; set; }
    }
}
