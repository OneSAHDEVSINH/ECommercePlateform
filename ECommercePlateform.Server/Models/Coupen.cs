using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommercePlateform.Server.Models
{
    public class Coupen
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("Product")]
        public Guid ProductId { get; set; }
        public virtual Product? Product { get; set; } // Navigation property to the Product entity
        [ForeignKey("ProductVarient")]
        public Guid ProductVarientId { get; set; }
        public virtual ProductVarient? ProductVarient { get; set; } // Navigation property to the ProductVarient entity

        [RegularExpression(@"^[A-Za-z0-9\s.,-]+$", ErrorMessage = "Only text and numbers are allowed.")]
        [DataType(DataType.Text)]
        [Display(Name = "Coupen Name:")]
        [DisplayFormat(ConvertEmptyStringToNull = true)]
        [Required(ErrorMessage = "Enter Coupen Name!")]
        [StringLength(100, ErrorMessage = "Coupen Name cannot be longer than 100 characters.")]
        public required string Name { get; set; }

        [RegularExpression(@"^[A-Za-z0-9\s.,-]+$", ErrorMessage = "Only text and numbers are allowed.")]
        [Display(Name = "Coupen Code:")]
        [DisplayFormat(ConvertEmptyStringToNull = true)]
        [Required(ErrorMessage = "Enter Coupen Code!")]
        [StringLength(100, ErrorMessage = "Coupen Code cannot be longer than 100 characters.")]
        [DataType(DataType.Text)]
        public required string Code { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Discount Type:")]
        [Required(ErrorMessage = "Select Discount Type!")]
        [DisplayFormat(ConvertEmptyStringToNull = true)]
        [StringLength(100, ErrorMessage = "Discount Type cannot be longer than 100 characters.")]
        [DefaultValue("FixedAmount")]
        public Enum.DiscountType DiscountType { get; set; } = Enum.DiscountType.FixedAmount; // e.g., Amount, Percentage

        [RegularExpression(@"/([0-9]*[\.]{0,1}[0-9]{0,2})/", ErrorMessage = "Enter valid value.")]
        [DataType(DataType.Text)]
        [Display(Name = "Discount Value:")]
        [Required(ErrorMessage = "Enter Discount Value!")]
        [Range(0.01, 9999999999.99, ErrorMessage = "Discount Value must be between 0.01 and 9999999999.99.")]
        [StringLength(20, ErrorMessage = "Discount Value cannot be longer than 20 characters.")]
        public required decimal DiscountValue { get; set; } = 0.0m;

        [RegularExpression(@"/([0-9]*[\.]{0,1}[0-9]{0,2})/", ErrorMessage = "Enter valid value.")]
        [DataType(DataType.Text)]
        [Display(Name = "Minimum Value:")]
        [Required(ErrorMessage = "Enter Minimum Value!")]
        [Range(0.01, 9999999999.99, ErrorMessage = "Minimum Value must be between 0.01 and 9999999999.99.")]
        [StringLength(20, ErrorMessage = "Minimum Value cannot be longer than 20 characters.")]
        [DefaultValue(0.0)]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true)]
        public decimal MinimumValue { get; set; }

        [RegularExpression(@"/([0-9]*[\.]{0,1}[0-9]{0,2})/", ErrorMessage = "Enter valid value.")]
        [DataType(DataType.Text)]
        [Display(Name = "Maximum Value:")]
        [Required(ErrorMessage = "Enter Maximum Value!")]
        [Range(0.01, 9999999999.99, ErrorMessage = "Maximum Value must be between 0.01 and 9999999999.99.")]
        [StringLength(20, ErrorMessage = "Maximum Value cannot be longer than 20 characters.")]
        [DefaultValue(0.0)]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true)]
        public decimal MaximumValue { get; set; }

        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Only numbers are allowed.")]
        [DataType(DataType.Text)]
        [Display(Name = "Minimum Quantity:")]
        [Required(ErrorMessage = "Enter Minimum Quantity!")]
        [Range(0, int.MaxValue, ErrorMessage = "Minimum Quantity must be a non-negative integer.")]
        [StringLength(100, ErrorMessage = "Minimum Quantity cannot be longer than 100 characters.")]
        [DefaultValue(0)]
        [DisplayFormat(DataFormatString = "{0:F0}", ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true)]
        public int MinimumQuantity { get; set; }

        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Only numbers are allowed.")]
        [DataType(DataType.Text)]
        [Display(Name = "Maximum Quantity:")]
        [Required(ErrorMessage = "Enter Maximum Quantity!")]
        [Range(0, int.MaxValue, ErrorMessage = "Maximum Quantity must be a non-negative integer.")]
        [StringLength(100, ErrorMessage = "Maximum Quantity cannot be longer than 100 characters.")]
        [DefaultValue(0)]
        [DisplayFormat(DataFormatString = "{0:F0}", ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true)]
        public int MaximumQuantity { get; set; }

        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Only numbers are allowed.")]
        [DataType(DataType.Text)]
        [Display(Name = "Validity Period:")]
        [Required(ErrorMessage = "Enter Validity Period!")]
        [Range(0, int.MaxValue, ErrorMessage = "Validity Period must be a non-negative integer.")]
        [StringLength(100, ErrorMessage = "Validity Period cannot be longer than 100 characters.")]
        [DefaultValue(0)]
        [DisplayFormat(DataFormatString = "{0:F0}", ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true)]
        public int ValidityPeriod { get; set; } // Validity period in days

        [Display(Name = "Validity Start Date:")]
        [Required(ErrorMessage = "Enter Validity Start Date!")]
        [StringLength(100, ErrorMessage = "Validity Start Date cannot be longer than 100 characters.")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true)]
        [DataType(DataType.DateTime)]
        public DateTime ValidityStartDate { get; set; } // Validity start date

        [Display(Name = "Validity End Date:")]
        [DataType(DataType.DateTime)]
        [StringLength(100, ErrorMessage = "Validity End Date cannot be longer than 100 characters.")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true)]
        [Required(ErrorMessage = "Enter Validity End Date!")]
        public DateTime ValidityEndDate { get; set; } // Validity end date

        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Only numbers are allowed.")]
        [Display(Name = "Total Use Count:")]
        [DataType(DataType.Text)]
        [StringLength(100, ErrorMessage = "Total Use Count cannot be longer than 100 characters.")]
        [Required(ErrorMessage = "Enter Total Use Count!")]
        [Range(0, int.MaxValue, ErrorMessage = "Total Use Count must be a non-negative integer.")]
        [DefaultValue(0)]
        [DisplayFormat(DataFormatString = "{0:F0}", ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true)]
        public int TotalUseCount { get; set; } = 0;// Total number of times the coupen can be used

        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Only numbers are allowed.")]
        [Display(Name = "Remaining Use Count:")]
        [DataType(DataType.Text)]
        [StringLength(100, ErrorMessage = "Remaining Use Count cannot be longer than 100 characters.")]
        [Required(ErrorMessage = "Enter Remaining Use Count!")]
        [Range(0, int.MaxValue, ErrorMessage = "Remaining Use Count must be a non-negative integer.")]
        [DefaultValue(0)]
        [DisplayFormat(DataFormatString = "{0:F0}", ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true)]
        public int RemainingUseCount { get; set; } = 0; // Remaining number of times the coupen can be used

        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Only numbers are allowed.")]
        [Display(Name = "Total Limited Use Count:")]
        [DataType(DataType.Text)]
        [StringLength(100, ErrorMessage = "Total Limited Use Count cannot be longer than 100 characters.")]
        [Required(ErrorMessage = "Enter Total Limited Use Count!")]
        [Range(0, int.MaxValue, ErrorMessage = "Total Limited Use Count must be a non-negative integer.")]
        [DefaultValue(0)]
        [DisplayFormat(DataFormatString = "{0:F0}", ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true)]
        public int TotalLimitedUseCount { get; set; } = 0;// Total number of times the coupen can be used

        public bool IsUsed { get; set; } // Indicates if the coupen has been used
        public bool IsExpired { get; set; } // Indicates if the coupen has expired
        public bool IsOneTimeUse { get; set; } // Indicates if the coupen can be used only once
        public bool IsLimitedUse { get; set; } // Indicates if the coupen has limited use
        public bool IsLimitedUsePerUser { get; set; } // Indicates if the coupen has limited use per user
        public bool IsLimitedUsePerProduct { get; set; } // Indicates if the coupen has limited use per product
        public bool IsLimitedUsePerProductVarient { get; set; } // Indicates if the coupen has limited use per product varient

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
        //public virtual ICollection<Coupen>? Coupens { get; set; } // Navigation property for related coupens
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
