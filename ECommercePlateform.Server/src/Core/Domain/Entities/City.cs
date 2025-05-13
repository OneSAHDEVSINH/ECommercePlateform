using System;
using System.Collections.Generic;

namespace ECommercePlateform.Server.Core.Domain.Entities
{
    public class City : BaseEntity
    {
        public required string Name { get; set; }
        public Guid StateId { get; set; }

        // Navigation properties
        public State? State { get; set; }
        public ICollection<Address>? Addresses { get; set; }
    }
} 