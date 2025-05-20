namespace ECommercePlatform.Application.DTOs
{
    public class CountryDto
    {
        public Guid Id { get; init; }
        public string? Name { get; init; }
        public string? Code { get; init; }
        public bool IsActive { get; init; }
        public List<StateDto>? States { get; init; }
    }

    public class CreateCountryDto
    {
        public required string Name { get; init; }
        public required string Code { get; init; }
    }

    public class UpdateCountryDto
    {
        public string? Name { get; init; }
        public string? Code { get; init; }
        public bool IsActive { get; init; }
    }
}