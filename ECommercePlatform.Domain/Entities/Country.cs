namespace ECommercePlatform.Domain.Entities
{
    public class Country : BaseEntity
    {
        public string? Name { get; private set; }
        public string? Code { get; private set; }

        // Navigation properties
        public ICollection<State>? States { get;  set; }
        public ICollection<Address>? Addresses { get;  set; }

        private Country() { }

        public static Country Create(string name, string code)
        {
            return new Country
            {
                Name = name,
                Code = code
            };
        }

        // Add a public method to update the state properties  
        public void Update(string name, string code)
        {
            Name = name;
            Code = code;
        }
    }
}