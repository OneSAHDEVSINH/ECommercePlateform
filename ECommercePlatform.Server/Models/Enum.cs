namespace ECommercePlatform.Server.Models
{
    public class Enum
    {
        public enum UserRole
        {
            Admin,
            Customer,
            Seller,
            Guest
        }
        public enum Gender
        {
            Male,
            Female,
            Other,
            Unknown
        }
        public enum AddressType
        {
            Home,
            Work,
            Billing,
            Shipping
        }
        public enum OrderStatus
        {
            Pending,
            Processing,
            Shipped,
            Delivered,
            Cancelled
        }
        public enum PaymentStatus
        {
            Pending,
            Completed,
            Failed,
            Refunded
        }
        public enum PaymentMethod
        {
            CreditCard,
            DebitCard,
            PayPal,
            BankTransfer,
            CashOnDelivery,
            GiftCard,
            Cryptocurrency,
            Other,
            UPI,
            NetBanking,
            Wallet
        }
        public enum ProductCategory
        {
            Electronics,
            Clothing,
            HomeAppliances,
            Books,
            BeautyProducts,
            SportsEquipment
        }
        public enum ProductStatus
        {
            Available,
            OutOfStock,
            Discontinued
        }
        public enum ReviewRating
        {
            OneStar = 1,
            TwoStars,
            ThreeStars,
            FourStars,
            FiveStars
        }
        public enum ShippingMethod
        {
            Standard,
            Express,
            Overnight,
            International
        }
        public enum WishlistStatus
        {
            Active,
            Inactive
        }
        public enum DeliveryStatus
        {
            Pending,
            OutForDelivery,
            Delivered,
            Returned
        }
        public enum CartStatus
        {
            Active,
            Inactive
        }
        public enum DeliveryMethod
        {
            Standard,
            Express,
            SameDay,
            Scheduled
        }
        public enum DiscountType
        {
            Percentage,
            FixedAmount,
            BuyOneGetOne,
            FreeShipping
        }
        public enum DiscountStatus
        {
            Active,
            Expired,
            Upcoming
        }
        public enum  SettingsType
        {
            Percentage,
            FixedAmount
        }
    }
}
