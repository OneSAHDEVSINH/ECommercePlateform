namespace ECommercePlatform.Application.DTOs
{
    public class StateDto
    {
        public Guid Id { get; init; }
        public string? Name { get; init; }
        public string? Code { get; init; }
        public Guid CountryId { get; init; }
        public string? CountryName { get; init; }
        public bool IsActive { get; init; }
        public List<CityDto>? Cities { get; init; }
    }

    public class CreateStateDto
    {
        public required string Name { get; init; }
        public required string Code { get; init; }
        public Guid CountryId { get; init; }
    }

    public class UpdateStateDto
    {
        public string? Name { get; init; }
        public string? Code { get; init; }
        public Guid CountryId { get; init; }
        public bool IsActive { get; init; }
    }
}