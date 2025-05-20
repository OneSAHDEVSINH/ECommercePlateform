namespace ECommercePlatform.Domain.Entities
{
    public class Country : BaseEntity
    {
        public string? Name { get;  set; }
        public string? Code { get;  set; }

        // Navigation properties
        public ICollection<State>? States { get;  set; }
        public ICollection<Address>? Addresses { get;  set; }

        public Country() { }
    }
}