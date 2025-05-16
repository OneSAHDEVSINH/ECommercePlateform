using System;
using System.Collections.Generic;

namespace ECommercePlatform.Server.Core.Domain.Entities
{
    public class Country : BaseEntity
    {
        public required string Name { get; set; }
        public string? Code { get; set; }

        // Navigation properties
        public ICollection<State>? States { get; set; }
        public ICollection<Address>? Addresses { get; set; }
    }
} 