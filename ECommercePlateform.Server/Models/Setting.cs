using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
namespace ECommercePlateform.Server.Models
{
    public class Setting
    {
        [Key]
        public Guid Id { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Tax Type:")]
        [Required(ErrorMessage = "Select Tax Type!")]
        [DisplayFormat(ConvertEmptyStringToNull = true)]
        [StringLength(100, ErrorMessage = "Tax Type cannot be longer than 100 characters.")]
        [DefaultValue("FixedAmount")]
        public Enum.SettingsType TaxType { get; set; } = Enum.SettingsType.FixedAmount; // e.g., Amount, Percentage

        [RegularExpression(@"/([0-9]*[\.]{0,1}[0-9]{0,2})/", ErrorMessage = "Enter valid rate.")]
        [DataType(DataType.Text)]
        [Display(Name = "Tax Rate:")]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true)]
        [StringLength(20, ErrorMessage = "Tax Rate cannot be longer than 20 characters.")]
        [DefaultValue(0.0)]
        [Required(ErrorMessage = "Enter Discount Value!")]
        [Range(0.01, 9999999999.99, ErrorMessage = "Discount Value must be between 0.01 and 9999999999.99.")]
        public required decimal TaxRate { get; set; } = 0.0m;

        [Display(Name = "Delivery Fee Type:")]
        [Required(ErrorMessage = "Select Delivery Fee Type!")]
        [DisplayFormat(ConvertEmptyStringToNull = true)]
        [StringLength(100, ErrorMessage = "Delivery Fee Type cannot be longer than 100 characters.")]
        [DefaultValue("FixedAmount")]
        public Enum.SettingsType DeliveryFeeType { get; set; } = Enum.SettingsType.FixedAmount; // e.g., Amount, Percentage

        [Display(Name = "Delivery Fee:")]
        [DataType(DataType.Text)]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true)]
        [StringLength(20, ErrorMessage = "Delivery Fee cannot be longer than 20 characters.")]
        [DefaultValue(0.0)]
        [Required(ErrorMessage = "Enter Delivery Fee!")]
        [Range(0.01, 9999999999.99, ErrorMessage = "Delivery Fee must be between 0.01 and 9999999999.99.")]
        [RegularExpression(@"/([0-9]*[\.]{0,1}[0-9]{0,2})/", ErrorMessage = "Enter valid delivery fee.")]
        public required decimal DeliveryFee { get; set; } = 0.0m;

        [Display(Name = "Free Delivery Above Type:")]
        [Required(ErrorMessage = "Select Free Delivery Above Type!")]
        [DisplayFormat(ConvertEmptyStringToNull = true)]
        [StringLength(100, ErrorMessage = "Free Delivery Above Type cannot be longer than 100 characters.")]
        [DefaultValue("FixedAmount")]
        public Enum.SettingsType FreeDeliveryAboveType { get; set; } = Enum.SettingsType.FixedAmount; // e.g., Amount, Percentage

        [Display(Name = "Free Delivery Above:")]
        [DataType(DataType.Text)]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true)]
        [StringLength(20, ErrorMessage = "Free Delivery Above cannot be longer than 20 characters.")]
        [DefaultValue(0.0)]
        [Required(ErrorMessage = "Enter Free Delivery Above!")]
        [Range(0.01, 9999999999.99, ErrorMessage = "Free Delivery Above must be between 0.01 and 9999999999.99.")]
        [RegularExpression(@"/([0-9]*[\.]{0,1}[0-9]{0,2})/", ErrorMessage = "Enter valid free delivery above.")]
        public required decimal FreeDeliveryAbove { get; set; } = 0.0m;

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
        //public virtual ICollection<Setting>? Settings { get; set; } // Navigation property for related settings
        public virtual ICollection<ShippingAddress>? ShippingAddresses { get; set; } // Navigation property for related shipping addresses
        public virtual ICollection<State>? States { get; set; } // Navigation property for related states
        public virtual ICollection<User>? Users { get; set; } // Navigation property for related users
    }
}
