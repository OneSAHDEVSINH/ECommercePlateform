namespace ECommercePlatform.Domain.Entities
{
    public class City : BaseEntity
    {
        public string? Name { get; private set; }
        public Guid StateId { get; private set; }

        // Navigation properties
        public virtual State? State { get; private set; }

        public virtual ICollection<Address>? Addresses { get; private set; }

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