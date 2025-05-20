namespace ECommercePlatform.Domain.Entities
{
    public class ShippingAddress : BaseEntity
    {
        public Guid UserId { get; private set; }
        public string? RecipientName { get; private set; }
        public string? PhoneNumber { get; private set; }
        public string? Line1 { get; private set; }
        public string? Line2 { get; private set; }
        public string? Line3 { get; private set; }
        public Guid CityId { get; private set; }
        public Guid StateId { get; private set; }
        public Guid CountryId { get; private set; }
        public string? ZipCode { get; private set; }
        public bool IsDefault { get; private set; }

        // Navigation properties
        public User? User { get; private set; }
        public City? City { get; private set; }
        public State? State { get; private set; }
        public Country? Country { get; private set; }
        public ICollection<Order>? Orders { get; private set; }

        private ShippingAddress() { }
    }
}