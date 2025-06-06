using ECommercePlatform.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommercePlatform.Domain.Entities
{
    public class User : BaseEntity
    {
        public string? FirstName { get; private set; }
        public string? LastName { get; private set; }
        public byte[]? Avatar { get; private set; }
        public Gender Gender { get; private set; }
        public DateOnly DateOfBirth { get; private set; }
        public string? PhoneNumber { get; private set; }
        public string? Email { get; private set; }
        public string? Password { get; private set; }
        public string? Bio { get; private set; }
        public UserRole? Role { get; private set; }
        public ICollection<Address>? Addresses { get; set; }
        public ICollection<Order>? Orders { get; set; }
        public ICollection<Review>? Reviews { get; set; }
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();


        // Replace 'With' method with a proper implementation  
        public User With(
            string? firstName = null,
            string? lastName = null,
            Gender? gender = null,
            DateOnly? dateOfBirth = null,
            string? phoneNumber = null,
            string? email = null,
            string? password = null,
            string? bio = null,
            UserRole? role = null
        )
        {
            return new User
            {
                Id = this.Id,
                CreatedOn = this.CreatedOn,
                CreatedBy = this.CreatedBy,
                ModifiedOn = this.ModifiedOn,
                ModifiedBy = this.ModifiedBy,
                IsActive = this.IsActive,
                IsDeleted = this.IsDeleted,
                FirstName = firstName ?? this.FirstName,
                LastName = lastName ?? this.LastName,
                Gender = gender ?? this.Gender,
                DateOfBirth = dateOfBirth ?? this.DateOfBirth,
                PhoneNumber = phoneNumber ?? this.PhoneNumber,
                Email = email ?? this.Email,
                Password = password ?? this.Password,
                Bio = bio ?? this.Bio,
                Role = role ?? this.Role,
                Addresses = this.Addresses,
                Orders = this.Orders,
                Reviews = this.Reviews
            };
        }

        // Ensure AdminCreate is implemented correctly  
        public static User AdminCreate(
            Guid id,
    string firstName,
    string lastName,
    Gender gender,
    DateOnly dateOfBirth,
    string phoneNumber,
    string email,
    string password,
    string bio,
    UserRole userRole,
    string createdBy,
    DateTime createdOn,
    bool isActive = true,
    bool isDeleted = false,
    string? modifiedBy = null,
    DateTime? modifiedOn = null
)
        {
            return new User
            {
                Id = id,
                FirstName = firstName,
                LastName = lastName,
                Gender = gender,
                DateOfBirth = dateOfBirth,
                PhoneNumber = phoneNumber,
                Email = email,
                Password = password,
                Bio = bio,
                Role = userRole,
                IsActive = isActive,
                IsDeleted = isDeleted,
                CreatedBy = createdBy,
                CreatedOn = createdOn,
                ModifiedBy = modifiedBy,
                ModifiedOn = modifiedOn ?? createdOn
            };
        }
    }
}