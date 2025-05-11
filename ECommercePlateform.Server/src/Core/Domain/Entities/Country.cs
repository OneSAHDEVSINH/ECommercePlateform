using System;
using System.Collections.Generic;

namespace ECommercePlateform.Server.Core.Domain.Entities
{
    public class Country
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public string? CreatedBy { get; set; }
        public DateTime ModifiedOn { get; set; } = DateTime.Now;
        public string? ModifiedBy { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; }

        // Navigation properties
        public ICollection<State>? States { get; set; }
        public ICollection<Address>? Addresses { get; set; }
    }
} 