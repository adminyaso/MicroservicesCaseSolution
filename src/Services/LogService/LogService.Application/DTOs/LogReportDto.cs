namespace LogService.Application.DTOs
{
    public class LogReportDto
    {
        public int TotalLogs { get; set; }
        public int InfoCount { get; set; }
        public int WarningCount { get; set; }
        public int ErrorCount { get; set; }
        public int CriticalCount { get; set; }
        public DateTime ReportGeneratedAt { get; set; }
    }
}