using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommercePlateform.Server.Models
{
    public class Review
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("Product")]
        public Guid ProductId { get; set; }

        public virtual Product? Product { get; set; } // Navigation property to the Product entity

        [ForeignKey("User")]
        public Guid UserId { get; set; }

        public virtual User? User { get; set; } // Navigation property to the User entity

        [Display(Name = "Rating:")]
        [DataType(DataType.Text)]
        [RegularExpression(@"^[1-5]$", ErrorMessage = "Rating must be between 1 and 5.")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        [Required(ErrorMessage = "Enter Rating!")]
        [StringLength(1, ErrorMessage = "Rating cannot be longer than 1 character.")]
        [DefaultValue(1)]
        [DisplayFormat(DataFormatString = "{0:F0}", ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true)]
        public int Rating { get; set; } = 1; // Rating given by the user

        [Display(Name = "Review:")]
        [DataType(DataType.MultilineText)]
        [MaxLength(255)]
        [DisplayFormat(ConvertEmptyStringToNull = true)]
        [Required(ErrorMessage = "Enter Review!")]
        [StringLength(255, ErrorMessage = "Review cannot be longer than 255 characters.")]
        [DefaultValue("General")]
        public required string ReviewText { get; set; } // Review text provided by the user

        [DefaultValue(false)]
        public bool IsApproved { get; set; } // Indicates whether the review is approved by an admin
        
        [DefaultValue(false)]
        public bool IsRejected { get; set; } // Indicates whether the review is rejected by an admin

        [DefaultValue(false)]
        public bool IsReported { get; set; } // Indicates whether the review is reported by a user

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
    }
}
