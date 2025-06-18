namespace ECommercePlatform.Domain.Entities
{
    public class City : BaseEntity
    {
        public string? Name { get; set; }
        public Guid StateId { get; set; }

        // Navigation properties
        public virtual State? State { get; set; }

        public virtual ICollection<Address>? Addresses { get; set; }

        private City() { }

        public static City Create(string name, Guid stateId)
        {
            return new City
            {
                Name = name,
                StateId = stateId
            };
        }

        // Add a public method to update the state properties  
        public void Update(string name, Guid stateId)
        {
            Name = name;
            StateId = stateId;
        }
    }
}