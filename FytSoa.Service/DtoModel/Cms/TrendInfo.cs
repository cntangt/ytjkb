using System;

namespace FytSoa.Service.DtoModel.Cms
{
    public class TrendInfo
    {
        public DateTime Day { get; set; }
        public int CountTrade { get; set; }
        public long TotalTrade { get; set; }
        public int CountRefund { get; set; }
        public long TotalRefund { get; set; }
    }
}
