using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WildberriesDeductions.Wildberries.Models
{
    public class DeductionGood
    {
        public long Shk { get; set; }
        public decimal DeductionPrice { get; set; }
        public DateTimeOffset? OnHoldDate { get; set; }

        public string Pickpoint { get; set; } = string.Empty;
        public long TicketId { get; set; }
        public string Reason { get; set; } = string.Empty;

        public GoodDetails? Details { get; set; } = null;

    }

    public class GoodDetails
    {
        public string Name { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public int Cell { get; set; }
        public decimal FullPrice { get; set; }
        public List<StatusHistoryRecord> StatusHistory { get; set; } = new();
        public List<DefectMarkData> DefectMark { get; set; } = new();


    }

    public class StatusHistoryRecord
    {
        public DateTimeOffset Date { get; set; }
        public int StatusId { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public string EmployeeFullName { get; set; } = string.Empty;
    }

    public class DefectMarkData
    {
        public DateTimeOffset Date { get; set; }
        public string Comment { get; set; } = string.Empty;
        public List<string> Photos { get; set; } = new();

    }
}
