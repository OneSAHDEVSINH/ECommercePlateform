namespace ECommercePlatform.Domain.Entities
{
    public class Address : BaseEntity
    {
        public string? Line1 { get; private set; }
        public string? Line2 { get; private set; }
        public string? Line3 { get; private set; }
        public Guid CityId { get; private set; }
        public Guid StateId { get; private set; }
        public Guid CountryId { get; private set; }
        public string? ZipCode { get; private set; }
        public Guid UserId { get; private set; }
        public bool IsDefault { get; private set; }

        // Navigation properties
        public virtual City? City { get; private set; }
        public virtual State? State { get; private set; }
        public virtual Country? Country { get; private set; }
        public virtual User? User { get; private set; }

        private Address() { }
    }
}