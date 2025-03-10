namespace LogService.Domain.Entities
{
    public class LogEntry
    {
        public int Id { get; set; }
        public DateTime? Timestamp { get; set; }
        public string? Message { get; set; } = string.Empty;
        public string? Level { get; set; } = string.Empty; // Örnek: INFO, WARNING, ERROR, FATAL, CRITICAL
        public string? Exception { get; set; }
        public string? Properties { get; set; } = string.Empty;
        public string? EnvironmentName { get; set; } = string.Empty;
        public string? MachineName { get; set; } = string.Empty;
        public string? ExceptionDetail { get; set; } = string.Empty;
        public string? Path { get; set; } = string.Empty;
        public string? RequestPath { get; set; } = string.Empty;
        public string? SourceContext { get; set; } = string.Empty;
    }
}