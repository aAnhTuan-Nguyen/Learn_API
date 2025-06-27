namespace TodoWeb.Domain.Entities
{
    public class AuditLog
    {
        public int Id { get; set; }
        public string EntityName { get; set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public string Action { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
