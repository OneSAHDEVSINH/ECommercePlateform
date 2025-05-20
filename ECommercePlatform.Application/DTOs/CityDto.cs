using ECommercePlatform.Application.Features.Cities.Commands.Update;
using ECommercePlatform.Application.Features.Countries.Commands.Update;
using ECommercePlatform.Domain.Entities;

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

        // Explicit conversion operator from Country to CountryDto
        public static explicit operator CityDto(City city)
        {
            return new CityDto
            {
                Id = city.Id,
                Name = city.Name,
                IsActive = city.IsActive,
                //Cities = state.Cities?.Select(city => (CityDto)city).ToList()
                //States = country.States?.Select(state => (StateDto)state).ToList()
            };
        }
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

        public static explicit operator UpdateCityDto(UpdateCityCommand command)
        {
            return new UpdateCityDto
            {
                Name = command.Name,
                IsActive = command.IsActive
            };
        }
    }
}