namespace Backend_Test_Task.Models
{
    public class ExceptionJournal
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string QueryParams { get; set; }
        public string BodyParams { get; set; }
        public string StackTrace { get; set; }
        public string ExceptionType { get; set; }
    }
}
