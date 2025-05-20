namespace ECommercePlatform.Domain.Entities
{
    public class Review : BaseEntity
    {
        public Guid ProductId { get; private set; }
        public Guid UserId { get; private set; }
        public int Rating { get; private set; }
        public string? Comment { get; private set; }

        // Navigation properties
        public Product? Product { get; private set; }
        public User? User { get; private set; }

        private Review() { }
    }
}