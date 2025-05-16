namespace ECommercePlatform.Domain.Exceptions
{
    public class DuplicateResourceException : Exception
    {
        public DuplicateResourceException() : base() { }
        public DuplicateResourceException(string message) : base(message) { }
        public DuplicateResourceException(string message, Exception innerException) : base(message, innerException) { }
    }
}
