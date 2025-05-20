namespace ECommercePlatform.Application.DTOs
{
    public class CityDto
    {
        public Guid Id { get; init; }
        public string? Name { get; init; }
        public Guid StateId { get; init; }
        public string? StateName { get; init; }
        public Guid CountryId { get; init; }
        public string? CountryName { get; init; }
        public bool IsActive { get; init; }
    }

    public class CreateCityDto
    {
        public required string Name { get; init; }
        public Guid StateId { get; init; }
    }

    public class UpdateCityDto
    {
        public string? Name { get; init; }
        public Guid StateId { get; init; }
        public bool IsActive { get; init; }
    }
}