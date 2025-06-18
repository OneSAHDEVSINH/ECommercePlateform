namespace ECommercePlatform.Domain.Entities
{
    public class ShippingAddress : BaseEntity
    {
        public Guid UserId { get; set; }
        public string? RecipientName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Line1 { get; set; }
        public string? Line2 { get; set; }
        public string? Line3 { get; set; }
        public Guid CityId { get; set; }
        public Guid StateId { get; set; }
        public Guid CountryId { get; set; }
        public string? ZipCode { get; set; }
        public bool IsDefault { get; set; }

        // Navigation properties
        public virtual User? User { get; set; }
        public virtual City? City { get; set; }
        public virtual State? State { get; set; }
        public virtual Country? Country { get; set; }
        public virtual ICollection<Order>? Orders { get; set; }

        private ShippingAddress() { }
    }
}