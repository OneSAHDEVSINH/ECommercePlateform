using System;
using System.Collections.Generic;

namespace ECommercePlateform.Server.Core.Domain.Entities
{
    public class State : BaseEntity
    {
        public required string Name { get; set; }
        public string? Code { get; set; }
        public Guid CountryId { get; set; }

        // Navigation properties
        public Country? Country { get; set; }
        public ICollection<City>? Cities { get; set; }
        public ICollection<Address>? Addresses { get; set; }
    }
} 