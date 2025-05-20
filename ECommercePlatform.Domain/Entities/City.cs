using FluentAssertions.Equivalency;

namespace ECommercePlatform.Domain.Entities
{
    public class City : BaseEntity
    {
        public string? Name { get; private set; }
        public Guid StateId { get; private set; }

        // Navigation properties
        public State? State { get;  set; }

        public ICollection<Address>? Addresses { get;  set; }

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
        public void Update(string name)
        {
            Name = name;
        }
    }
}