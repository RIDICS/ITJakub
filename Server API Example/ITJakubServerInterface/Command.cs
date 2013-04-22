namespace ITJakubServerInterface
{
    public class Command
    {
        public long Id { get; set; }

        public long UserId { get; set; }

        public long SessionId { get; set; }

        public string CommandText { get; set; }
    }
}