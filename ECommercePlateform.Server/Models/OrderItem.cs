using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommercePlateform.Server.Models
{
    public class OrderItem
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("Order")]
        public Guid OrderId { get; set; }

        public virtual Order? Order { get; set; } // Navigation property to the Order entity

        [ForeignKey("Product")]
        public Guid ProductId { get; set; }

        public virtual Product? Product { get; set; } // Navigation property to the Product entity

        [ForeignKey("ProductVarient")]
        public Guid ProductVarientId { get; set; }

        public virtual ProductVarient? ProductVarient { get; set; } // Navigation property to the ProductVarient entity

        [Display(Name = "Product Name:")]
        [DataType(DataType.Text)]
        [DisplayFormat(ConvertEmptyStringToNull = true)]
        [Required(ErrorMessage = "Enter Product Name!")]
        [StringLength(100, ErrorMessage = "Product Name cannot be longer than 100 characters.")]
        [RegularExpression(@"^[A-Za-z0-9\s.,-]+$", ErrorMessage = "Only text and numbers are allowed.")]
        public required string ProductName { get; set; } // Name of the product in the order

        [Display(Name = "SKU:")]
        [DataType(DataType.Text)]
        [DisplayFormat(ConvertEmptyStringToNull = true)]
        [StringLength(50, ErrorMessage = "SKU cannot be longer than 50 characters.")]
        [RegularExpression(@"^[A-Za-z0-9\s.,-]+$", ErrorMessage = "Only text and numbers are allowed.")]
        public string? SKU { get; set; } // Stock Keeping Unit of the product in the order

        [Display(Name = "Quantity:")]
        [DataType(DataType.Text)]
        [RegularExpression(@"^\d+$", ErrorMessage = "Quantity must be a non-negative integer.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be a positive integer.")]
        [Required(ErrorMessage = "Enter Quantity!")]
        [StringLength(10, ErrorMessage = "Quantity cannot be longer than 10 characters.")]
        [DefaultValue(1)]
        public int Quantity { get; set; } = 1; // Quantity of the product in the order

        [Display(Name = "Unit Price:")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true)]
        [Required(ErrorMessage = "Enter Unit Price!")]
        [Range(0.01, 9999999999.99, ErrorMessage = "Unit Price must be between 0.01 and 9999999999.99.")]
        [StringLength(20, ErrorMessage = "Unit Price cannot be longer than 20 characters.")]
        [DefaultValue(0.0)]
        [RegularExpression(@"/([0-9]*[\.]{0,1}[0-9]{0,2})/", ErrorMessage = "Enter valid unit price.")]
        public decimal UnitPrice { get; set; } = 0.0m; // Unit price of the product

        [Display(Name = "Total Price:")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true)]
        [Required(ErrorMessage = "Enter Total Price!")]
        [Range(0.01, 9999999999.99, ErrorMessage = "Total Price must be between 0.01 and 9999999999.99.")]
        [StringLength(20, ErrorMessage = "Total Price cannot be longer than 20 characters.")]
        [DefaultValue(0.0)]
        [RegularExpression(@"/([0-9]*[\.]{0,1}[0-9]{0,2})/", ErrorMessage = "Enter valid total price.")]
        public decimal TotalPrice { get; set; } = 0.0m; // Total price of the product in the order

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
        //public virtual ICollection<OrderItem>? OrderItems { get; set; } // Navigation property for related order items
        public virtual ICollection<Product>? Products { get; set; } // Navigation property for related products
        public virtual ICollection<ProductVarient>? ProductVarients { get; set; } // Navigation property for related product varients
        public virtual ICollection<Review>? Reviews { get; set; } // Navigation property for related reviews
        public virtual ICollection<Setting>? Settings { get; set; } // Navigation property for related settings
        public virtual ICollection<ShippingAddress>? ShippingAddresses { get; set; } // Navigation property for related shipping addresses
        public virtual ICollection<State>? States { get; set; } // Navigation property for related states
        public virtual ICollection<User>? Users { get; set; } // Navigation property for related users
    }
}
