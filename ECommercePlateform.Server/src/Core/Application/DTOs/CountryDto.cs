using System;
using System.Collections.Generic;

namespace ECommercePlateform.Server.Core.Application.DTOs
{
    public class CountryDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }
        public bool IsActive { get; set; }
        public List<StateDto>? States { get; set; }
    }

    public class CreateCountryDto
    {
        public required string Name { get; set; }
        public required string Code { get; set; }
    }

    public class UpdateCountryDto
    {
        public string? Name { get; set; }
        public string? Code { get; set; }
        public bool IsActive { get; set; }
    }
} 