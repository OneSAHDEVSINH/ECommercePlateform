using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;

namespace ECommercePlateform.Server.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }

        [StringLength(50)]
        [Required(ErrorMessage = "Enter First Name!")]
        [Display(Name = "First Name:")]
        [RegularExpression(@"^[A-Za-z]+$", ErrorMessage = "Only text is allowed.")]
        [DataType(DataType.Text)]
        public required string FirstName { get; set; }

        [Required(ErrorMessage = "Enter Last Name!")]
        [Display(Name = "Last Name:")]
        [RegularExpression(@"^[A-Za-z]+$", ErrorMessage = "Only text is allowed.")]
        [StringLength(50)]
        [DataType(DataType.Text)]
        public required string LastName { get; set; }

        [Display(Name = "Profile Image:")]
        [DataType(DataType.Upload)]
        [FileExtensions(Extensions = "jpg,jpeg,gif,png,svg,jfif", ErrorMessage = "Only JPEG and GIF images are allowed.")]
        public byte[]? Avatar { get; set; }

        [Display(Name = "Gender:")]
        [Required(ErrorMessage = "Select Gender!")]
        public required Enum.Gender Gender { get; set; }

        [Required(ErrorMessage = "Select Date of Birth!")]
        [Display(Name = "Date of Birth:")]
        [DataType(DataType.Date)]
        public required DateOnly DateOfBirth { get; set; }

        [Required]
        [RegularExpression(@"^\d{10,15}$", ErrorMessage = "Phone number must be between 10 and 15 digits.")]
        [DataType(DataType.PhoneNumber)]
        [Phone]
        [Display(Name = "Phone Number")]
        [StringLength(15, ErrorMessage = "Phone number must be between 10 and 15 digits.", MinimumLength = 10)]
        public required string PhoneNumber { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(254, ErrorMessage = "Email address cannot be longer than 254 characters.")]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid email address format.")]
        public required string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password:")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$", ErrorMessage = "Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, one digit, and one special character.")]
        [StringLength(100, ErrorMessage = "Password must be at least 6 characters long.", MinimumLength = 6)]
        public required string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password!")]
        [Display(Name = "Confirm Password:")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public required string ConfirmPassword { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Bio:")]
        public string? Bio { get; set; }

        [Required]
        public required Enum.UserRole Role { get; set; } = Enum.UserRole.Customer;

        [DataType(DataType.DateTime)]
        public DateTime CreatedOn { get; set; } = DateTime.Now;

        public string? CreatedBy { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime ModifiedOn { get; set; } = DateTime.Now;

        public string? ModifiedBy { get; set; }

        public bool IsActive { get; set; } = true;

        public bool IsDeleted { get; set; }

        // Navigation properties for related entities
        public virtual ICollection<Address>? Addresses { get; set; } // Navigation property for related addresses
        public virtual ICollection<Cart>? Carts { get; set; } // Navigation property for related carts
        public virtual ICollection<CartItem>? CartItems { get; set; } // Navigation property for related cart items
        public virtual ICollection<City>? Cities { get; set; } // Navigation property for related cities
        public virtual ICollection<Country>? Countries { get; set; } // Navigation property for related countries
        public virtual ICollection<Coupen>? Coupens { get; set; } // Navigation property for related coupens
        public virtual ICollection<Order>? Orders { get; set; } // Navigation property for related orders
        public virtual ICollection<OrderItem>? OrderItems { get; set; } // Navigation property for related order items
        public virtual ICollection<Product>? Products { get; set; } // Navigation property for related products
        public virtual ICollection<ProductVarient>? ProductVarients { get; set; } // Navigation property for related product varients
        public virtual ICollection<Review>? Reviews { get; set; } // Navigation property for related reviews
        public virtual ICollection<Setting>? Settings { get; set; } // Navigation property for related settings
        public virtual ICollection<ShippingAddress>? ShippingAddresses { get; set; } // Navigation property for related shipping addresses
        public virtual ICollection<State>? States { get; set; } // Navigation property for related states
        //public virtual ICollection<User>? Users { get; set; } // Navigation property for related users

    }
}
