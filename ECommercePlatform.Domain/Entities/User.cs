using ECommercePlatform.Domain.Enums;

namespace ECommercePlatform.Domain.Entities
{
    public class User : BaseEntity
    {
        public string? FirstName { get;  set; }
        public string? LastName { get;  set; }
        public byte[]? Avatar { get;  set; }
        public Gender Gender { get;  set; }
        public DateOnly DateOfBirth { get;  set; }
        public string? PhoneNumber { get;  set; }
        public string? Email { get;  set; }
        public string? Password { get;  set; }
        public string? Bio { get;  set; }
        public UserRole Role { get;  set; } = UserRole.Customer;

        // Navigation properties - we'll define these relationships but avoid circular references
        public ICollection<Address>? Addresses { get;  set; }
        public ICollection<Order>? Orders { get;  set; }
        public ICollection<Review>? Reviews { get;  set; }

        public User() { }
    }
}