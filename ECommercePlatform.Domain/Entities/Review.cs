namespace ECommercePlatform.Domain.Entities
{
    public class Review : BaseEntity
    {
        public Guid ProductId { get; set; }
        public Guid UserId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }

        // Navigation properties
        public virtual Product? Product { get; set; }
        public virtual User? User { get; set; }

        private Review() { }
    }
}