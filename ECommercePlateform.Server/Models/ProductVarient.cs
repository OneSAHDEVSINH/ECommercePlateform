using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommercePlateform.Server.Models
{
    public class ProductVarient
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("Product")]
        public Guid ProductId { get; set; }
        public virtual Product? Product { get; set; } // Navigation property to the Product entity
        
        [Display(Name = "Product Varient Name:")]
        [RegularExpression(@"^[A-Za-z0-9\s.,-]+$", ErrorMessage = "Only text and numbers are allowed.")]
        [DataType(DataType.Text)]
        [DisplayFormat(ConvertEmptyStringToNull = true)]
        [Required(ErrorMessage = "Enter Product Varient Name!")]
        public required string Name { get; set; }

        [Display(Name = "Color:")]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Only text is allowed.")]
        [DataType(DataType.Text)]
        [DisplayFormat(ConvertEmptyStringToNull = true)]
        [StringLength(50, ErrorMessage = "Color cannot be longer than 50 characters.")]
        [DefaultValue("General")]
        public string? Color { get; set; } // Color of the product varient

        [Display(Name = "Size:")]
        [DataType(DataType.Text)]
        [DisplayFormat(ConvertEmptyStringToNull = true)]
        [StringLength(50, ErrorMessage = "Size cannot be longer than 50 characters.")]
        [DefaultValue("General")]
        [RegularExpression(@"^[A-Za-z0-9\s.,-]+$", ErrorMessage = "Only text and numbers are allowed.")]
        public string? Size { get; set; } // Size of the product varient

        [DataType(DataType.MultilineText)]
        [Display(Name = "Discription:")]
        [DisplayFormat(ConvertEmptyStringToNull = true)]
        [StringLength(500, ErrorMessage = "Discription cannot be longer than 500 characters.")]
        public string? Description { get; set; } = string.Empty; // Description of the product

        [RegularExpression(@"/([0-9]*[\.]{0,1}[0-9]{0,2})/", ErrorMessage = "Enter valid price.")]
        [DataType(DataType.Currency)]
        [Display(Name = "Price:")]
        [Required(ErrorMessage = "Enter Price!")]
        [Range(0.01, 9999999999.99, ErrorMessage = "Price must be between 0.01 and 9999999999.99.")]
        [StringLength(20, ErrorMessage = "Price cannot be longer than 20 characters.")]
        [DefaultValue(0.0)]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true)]
        public required decimal Price { get; set; } = 0.0m; // Base price of the product

        [Display(Name = "Product Image:")]
        [DataType(DataType.Upload)]
        [FileExtensions(Extensions = "jpg,jpeg,gif,svg,png,jfif", ErrorMessage = "Only JPEG and GIF images are allowed.")]
        public byte[]? Image { get; set; }

        [Display(Name = "Product Image URL:")]
        [DataType(DataType.Url)]
        [DisplayFormat(ConvertEmptyStringToNull = true)]
        [StringLength(500, ErrorMessage = "Product Image URL cannot be longer than 500 characters.")]
        [RegularExpression(@"^(https?|ftp)://[^\s/$.?#].[^\s]*$", ErrorMessage = "Invalid URL format.")]
        public string? ImageUrl { get; set; } // URL of the product image

        [Required]
        [Display(Name = "Stock of varient:")]
        [Range(0, int.MaxValue, ErrorMessage = "Stock must be a non-negative integer.")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Stock must be a non-negative integer.")]
        [DataType(DataType.Text)]
        [StringLength(10, ErrorMessage = "Stock cannot be longer than 10 characters.")]
        [DefaultValue(0)]
        [DisplayFormat(DataFormatString = "{0:F0}", ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true)]
        public required int StockQuantity { get; set; } = 0; // Stock quantity of the product

        [Display(Name = "SKU:")]
        [DataType(DataType.Text)]
        [DisplayFormat(ConvertEmptyStringToNull = true)]
        [StringLength(50, ErrorMessage = "SKU cannot be longer than 50 characters.")]
        [RegularExpression(@"^[A-Za-z0-9-]+$", ErrorMessage = "Only alphanumeric characters and hyphens are allowed.")]
        public string? SKU { get; set; } // Stock Keeping Unit

        [Display(Name = "Tags:")]
        [DataType(DataType.Text)]
        [StringLength(100, ErrorMessage = "Tags cannot be longer than 100 characters.")]
        [RegularExpression(@"^[A-Za-z0-9\s,]+$", ErrorMessage = "Only alphanumeric characters and spaces are allowed.")]
        [DefaultValue("General")]
        [DisplayFormat(DataFormatString = "{0:F0}", ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true)]
        public string? Tags { get; set; } = string.Empty;

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
        //public virtual ICollection<ProductVarient>? ProductVarients { get; set; } // Navigation property for related product varients
        public virtual ICollection<Review>? Reviews { get; set; } // Navigation property for related reviews
        public virtual ICollection<Setting>? Settings { get; set; } // Navigation property for related settings
        public virtual ICollection<ShippingAddress>? ShippingAddresses { get; set; } // Navigation property for related shipping addresses
        public virtual ICollection<State>? States { get; set; } // Navigation property for related states
        public virtual ICollection<User>? Users { get; set; } // Navigation property for related users
    }
}
