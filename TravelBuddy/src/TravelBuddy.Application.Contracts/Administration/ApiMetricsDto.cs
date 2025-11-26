namespace TravelBuddy.Administration
{
    public class ApiMetricsDto
    {
        public int TotalRequestsToday { get; set; }
        public int SuccessfulRequests { get; set; }
        public int FailedRequests { get; set; }
        public double AverageDurationMs { get; set; }
    }
}