using ECommercePlatform.Application.Features.Countries.Commands.Update;
using ECommercePlatform.Application.Features.States.Commands.Update;
using ECommercePlatform.Domain.Entities;

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

        // Explicit conversion operator from Country to CountryDto
        public static explicit operator StateDto(State state)
        {
            return new StateDto
            {
                Id = state.Id,
                Name = state.Name,
                Code = state.Code,
                IsActive = state.IsActive,
                Cities = state.Cities?.Select(city => (CityDto)city).ToList()
            };
        }
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

        public static explicit operator UpdateStateDto(UpdateStateCommand command)
        {
            return new UpdateStateDto
            {
                Name = command.Name,
                Code = command.Code,
                IsActive = command.IsActive
            };
        }
    }
}