using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommercePlateform.Server.Models
{
    public class Cart
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey("User")]
        public Guid UserId { get; set; }
        public virtual User? User { get; set; } // Navigation property to the User entity

        [Display(Name = "Total Items:")]
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Enter Total Items!")]
        [Range(0, 9999999999, ErrorMessage = "Total Items must be between 0 and 9999999999.")]
        [StringLength(10, ErrorMessage = "Total Items cannot be longer than 10 characters.")]
        [DefaultValue(0)]
        [DisplayFormat(DataFormatString = "{0:F0}", ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true)]
        [RegularExpression(@"^\d+$", ErrorMessage = "Total Items must be a non-negative integer.")]
        public int TotalItems { get; set; } = 0; // Total number of items in the cart

        [Display(Name = "Total Amount:")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true)]
        [Required(ErrorMessage = "Enter Total Amount!")]
        [Range(0.01, 9999999999.99, ErrorMessage = "Total Amount must be between 0.01 and 9999999999.99.")]
        [StringLength(20, ErrorMessage = "Total Amount cannot be longer than 20 characters.")]
        [DefaultValue(0.0)]
        [RegularExpression(@"/([0-9]*[\.]{0,1}[0-9]{0,2})/", ErrorMessage = "Enter valid total amount.")]
        public decimal TotalAmount { get; set; } = 0.0m; // Total price of the items in the cart

        [Required]
        [DataType(DataType.DateTime)]
        public required DateTime CreatedOn { get; set; } = DateTime.Now;

        [Required]
        public required string CreatedBy { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public required DateTime ModifiedOn { get; set; } = DateTime.Now;

        [Required]
        public required string ModifiedBy { get; set; }

        [Required]
        public required bool IsActive { get; set; } = true;

        [Required]
        public required bool IsDeleted { get; set; }

        // Navigation properties for related entities
        public virtual ICollection<Address>? Addresses { get; set; } // Navigation property for related addresses
        //public virtual ICollection<Cart>? Carts { get; set; } // Navigation property for related carts
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
    }
}
