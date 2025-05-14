namespace ECommercePlateform.Domain.Entities
{
    public class Review : BaseEntity
    {
        public Guid ProductId { get; set; }
        public Guid UserId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }

        // Navigation properties
        public Product? Product { get; set; }
        public User? User { get; set; }
    }
}