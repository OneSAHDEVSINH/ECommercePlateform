namespace ECommercePlatform.Application.DTOs
{
    public class CityDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public Guid StateId { get; set; }
        public string? StateName { get; set; }
        public Guid CountryId { get; set; }
        public string? CountryName { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateCityDto
    {
        public required string Name { get; set; }
        public Guid StateId { get; set; }
    }

    public class UpdateCityDto
    {
        public string? Name { get; set; }
        public Guid StateId { get; set; }
        public bool IsActive { get; set; }
    }
}