using System.Text.Json.Serialization;

namespace WildberriesDeductions.Wildberries.Models.Dto
{
    public class WbTicketsResponse
    {
        [JsonPropertyName("data")]
        public List<TicketDto> Data { get; set; } = new();
    }

    public class TicketDto
    {
        [JsonPropertyName("support_logs")]
        public List<TicketHistory> Logs { get; set; } = new();

        public string Reason { get; set; } = "Тикет";
        public string Pickpoint { get; set; } = "";
        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("id")]
        public long Id { get; set; }
        [JsonPropertyName("ticket_state_id")]
        public int State { get; set; }
        [JsonPropertyName("user_id")]
        public long UserId { get; set; }
        [JsonPropertyName("create_date")]
        public string CreateDateRaw { get; set; } = string.Empty;
        public DateTimeOffset? CreateDate => String.IsNullOrEmpty(CreateDateRaw) ? null : DateTimeOffset.Parse(CreateDateRaw).LocalDateTime;
        [JsonPropertyName("finish_date")]
        public string FinishDateRaw { get; set; } = string.Empty;
        public DateTimeOffset? FinishDate => String.IsNullOrEmpty(CreateDateRaw) ? null : DateTimeOffset.Parse(CreateDateRaw).LocalDateTime;
        [JsonPropertyName("ticket_type_name")]
        public string TypeName { get; set; } = string.Empty;

    }
    public class TicketHistory
    {
        [JsonPropertyName("create_date")]
        public string CreateDateRaw { get; set; } = string.Empty;
        public DateTimeOffset? CreateDate => String.IsNullOrEmpty(CreateDateRaw) ? null : DateTimeOffset.Parse(CreateDateRaw).LocalDateTime;
        [JsonPropertyName("fio")]
        public string FullName { get; set; } = string.Empty;
        [JsonPropertyName("user_id")]
        public long UserId { get; set; }
        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;
    }
}