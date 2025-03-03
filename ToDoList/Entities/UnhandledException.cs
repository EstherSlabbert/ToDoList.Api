namespace ToDoList.Entities
{
    public class UnhandledException
    {
        public int Id { get; set; }
        public required string Message { get; set; }
        public required string StackTrace { get; set; }
        public required string Source { get; set; }
        public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
        public string? RequestPath { get; set; }
        public string? RequestMethod { get; set; }
    }
}
