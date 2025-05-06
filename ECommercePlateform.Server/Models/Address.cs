using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;

namespace ECommercePlateform.Server.Models
{
    public class Address
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("User")]
        public Guid UserId { get; set; }
        public virtual User? User { get; set; } // Navigation property to the User entity

        [Display(Name = "Address Line 1:")]
        [RegularExpression(@"^[A-Za-z0-9\s.,-]+$", ErrorMessage = "Only text and numbers are allowed.")]
        [DataType(DataType.MultilineText)]
        [DisplayFormat(ConvertEmptyStringToNull = true)]
        [Required(ErrorMessage = "Enter Address Line 1!")]
        [StringLength(100, ErrorMessage = "Address Line 1 cannot be longer than 100 characters.")]
        public required string Line1 { get; set; }

        [Display(Name = "Address Line 2:")]
        [RegularExpression(@"^[A-Za-z0-9\s.,-]+$", ErrorMessage = "Only text and numbers are allowed.")]
        [DataType(DataType.MultilineText)]
        [DisplayFormat(ConvertEmptyStringToNull = true)]
        [StringLength(100, ErrorMessage = "Address Line 1 cannot be longer than 100 characters.")]
        public string? Line2 { get; set; }

        [Display(Name = "Address Line 3:")]
        [RegularExpression(@"^[A-Za-z0-9\s.,-]+$", ErrorMessage = "Only text and numbers are allowed.")]
        [DataType(DataType.MultilineText)]
        [DisplayFormat(ConvertEmptyStringToNull = true)]
        [StringLength(100, ErrorMessage = "Address Line 1 cannot be longer than 100 characters.")]
        public string? Line3 { get; set; }

        [ForeignKey("City")]
        public Guid CityId { get; set; }
        public virtual City? City { get; set; } // Navigation property to the City entity

        [ForeignKey("State")]
        public Guid StateId { get; set; }
        public virtual State? State { get; set; } // Navigation property to the State entity

        [ForeignKey("Country")]
        public Guid CountryId { get; set; }
        public virtual Country? Country { get; set; } // Navigation property to the Country entity

        [Required]
        [RegularExpression(@"^[A-Za-z0-9\s.,-]+$", ErrorMessage = "Only text and numbers are allowed.")]
        [DataType(DataType.PostalCode)]
        [Display(Name = "City:")]
        [StringLength(18, ErrorMessage = "ZipCode cannot be longer than 18 characters.")]
        public required string ZipCode { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Address Type:")]
        [DisplayFormat(ConvertEmptyStringToNull = true)]
        [StringLength(100, ErrorMessage = "Address Type cannot be longer than 100 characters.")]
        [DefaultValue("Home")]
        public required Enum.AddressType AddressType { get; set; } = Enum.AddressType.Home; // e.g., Home, Work, etc.

        public bool IsDefault { get; set; } // Indicates if this is the default address for the user

        [DataType(DataType.DateTime)]
        public DateTime CreatedOn { get; set; } = DateTime.Now;

        public string? CreatedBy { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime ModifiedOn { get; set; } = DateTime.Now;

        public string? ModifiedBy { get; set; }

        public bool IsActive { get; set; } = true;

        public bool IsDeleted { get; set; }

        // Add backing field for Addresses collection
        //private ICollection<Address>? _addresses;
        //public virtual ICollection<Address>? Addresses
        //{
        //    get => _addresses ?? new List<Address>();
        //    set => _addresses = value;
        //}

        // Navigation properties for related entities

        //public virtual ICollection<Address>? Addresses { get; set; } // Navigation property for related addresses
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
        public virtual ICollection<User>? Users { get; set; } // Navigation property for related users

        public override string ToString()
        {
            return $"{Line1}, {CityId}, {StateId}, {CountryId}, {ZipCode}";
        }
        public string GetFullAddress()
        {
            return $"{Line1}, {Line2}, {CityId}, {StateId}, {CountryId}, {ZipCode}";
        }
        public string GetFullAddressWithType()
        {
            return $"{GetFullAddress()}, Type: {AddressType}";
        }
    }
}
