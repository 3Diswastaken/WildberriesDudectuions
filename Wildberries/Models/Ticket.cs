using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WildberriesDeductions.Wildberries.Models
{
    public class Ticket
    {
        public string PickpointName { get; set; } = string.Empty;
        public string ShortPickpointName { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty; // сообщение с которым был создан тикет
        public long Id { get; set; }
        public DateTimeOffset? CreateDate { get; set; }
        public DateTimeOffset? FinishDate { get; set; }
        public DateTimeOffset? OnHoldDate { get; set; }
        public int State { get; set; }
        public string TypeName { get; set; } = string.Empty;
        public List<TicketHistoryRecord> Logs { get; set; } = new();
    }

    public class TicketHistoryRecord
    {
        public DateTimeOffset? CreateDate { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
