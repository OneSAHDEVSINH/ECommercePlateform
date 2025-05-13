using System;

namespace ECommercePlateform.Server.Core.Domain.Entities
{
    public class Address : BaseEntity
    {
        public required string Line1 { get; set; }
        public string? Line2 { get; set; }
        public string? Line3 { get; set; }
        public Guid CityId { get; set; }
        public Guid StateId { get; set; }
        public Guid CountryId { get; set; }
        public string? ZipCode { get; set; }
        public Guid UserId { get; set; }
        public bool IsDefault { get; set; }

        // Navigation properties
        public City? City { get; set; }
        public State? State { get; set; }
        public Country? Country { get; set; }
        public User? User { get; set; }
    }
} 