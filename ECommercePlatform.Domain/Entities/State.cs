namespace ECommercePlatform.Domain.Entities
{
    public class State : BaseEntity
    {
        public string? Name { get; set; }
        public string? Code { get; set; }
        public Guid CountryId { get; set; }

        //Navigation properties
        public virtual Country? Country { get; set; }
        public virtual ICollection<City>? Cities { get; set; }
        public virtual ICollection<Address>? Addresses { get; set; }

        private State() { }

        public static State Create(string name, string code, Guid countryId)
        {
            return new State
            {
                Name = name,
                Code = code,
                CountryId = countryId
            };
        }

        // Add a public method to update the state properties  
        public void Update(string name, string code, Guid countryId)
        {
            Name = name;
            Code = code;
            CountryId = countryId;
        }
    }
}