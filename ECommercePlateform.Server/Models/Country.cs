using System.ComponentModel.DataAnnotations;

namespace ECommercePlateform.Server.Models
{
    public class Country
    {
        [Key]
        public Guid Id { get; set; }

        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Only text is allowed.")]
        [DataType(DataType.Text)]
        [Display(Name = "Country Name:")]
        [DisplayFormat(ConvertEmptyStringToNull = true)]
        [Required(ErrorMessage = "Enter Country Name!")]
        [StringLength(100, ErrorMessage = "Country Name cannot be longer than 100 characters.")]
        public required string Name { get; set; }

        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Only text is allowed.")]
        [DataType(DataType.Text)]
        [Display(Name = "Country Code:")]
        [DisplayFormat(ConvertEmptyStringToNull = true)]
        [Required(ErrorMessage = "Enter Country Code!")]
        [StringLength(10, ErrorMessage = "Country Code cannot be longer than 10 characters.")]
        public required string Code { get; set; }

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
        //public virtual ICollection<Country>? Countries { get; set; } // Navigation property for related countries
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

    }
}
