namespace ECommercePlatform.Domain.Entities
{
    public class Address : BaseEntity
    {
        public string? Line1 { get; set; }
        public string? Line2 { get; set; }
        public string? Line3 { get; set; }
        public Guid CityId { get; set; }
        public Guid StateId { get; set; }
        public Guid CountryId { get; set; }
        public string? ZipCode { get; set; }
        public Guid UserId { get; set; }
        public bool IsDefault { get; set; }

        // Navigation properties
        public virtual City? City { get; set; }
        public virtual State? State { get; set; }
        public virtual Country? Country { get; set; }
        public virtual User? User { get; set; }

        private Address() { }
    }
}