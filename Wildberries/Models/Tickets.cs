using System.Text.Json.Serialization;

namespace WildberriesDeductions.Wildberries.Models
{
    public class WbTicketsResponse
    {
        [JsonPropertyName("data")]
        public List<Ticket> Data { get; set; } = new();
    }

    public class Ticket
    {
        [JsonPropertyName("support_logs")]
        public List<TicketHistory> Logs { get; set; } = new();

        public string Reason { get; set; } = "Тикет";
        public string Pickpoint { get; set; } = "";
   
        [JsonPropertyName("id")]
        public long TicketId { get; set; }
        [JsonPropertyName("ticket_state_id")]
        public int State { get; set; }
    }
    public class TicketHistory
    {
        [JsonPropertyName("create_date")]
        public string CreateDateRaw { get; set; } = "";
        public DateTimeOffset? CreateDate => String.IsNullOrEmpty(CreateDateRaw) ? null : DateTimeOffset.Parse(CreateDateRaw).LocalDateTime;
        [JsonPropertyName("fio")]
        public string FullName { get; set; } = "";
    }
}