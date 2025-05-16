using ECommercePlatform.Domain.Enums;

namespace ECommercePlatform.Domain.Entities
{
    public class User : BaseEntity
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public byte[]? Avatar { get; set; }
        public Gender Gender { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public string? Bio { get; set; }
        public UserRole Role { get; set; } = UserRole.Customer;

        // Navigation properties - we'll define these relationships but avoid circular references
        public ICollection<Address>? Addresses { get; set; }
        public ICollection<Order>? Orders { get; set; }
        public ICollection<Review>? Reviews { get; set; }
    }
}