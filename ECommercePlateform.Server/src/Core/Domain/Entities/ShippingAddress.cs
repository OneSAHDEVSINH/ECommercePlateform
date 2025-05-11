using System;
using System.Collections.Generic;

namespace ECommercePlateform.Server.Core.Domain.Entities
{
    public class ShippingAddress
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string RecipientName { get; set; }
        public string PhoneNumber { get; set; }
        public string Line1 { get; set; }
        public string? Line2 { get; set; }
        public string? Line3 { get; set; }
        public Guid CityId { get; set; }
        public Guid StateId { get; set; }
        public Guid CountryId { get; set; }
        public string ZipCode { get; set; }
        public bool IsDefault { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public string? CreatedBy { get; set; }
        public DateTime ModifiedOn { get; set; } = DateTime.Now;
        public string? ModifiedBy { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; }

        // Navigation properties
        public User User { get; set; }
        public City City { get; set; }
        public State State { get; set; }
        public Country Country { get; set; }
        public ICollection<Order>? Orders { get; set; }
    }
} 