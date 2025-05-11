using System;
using System.Collections.Generic;

namespace ECommercePlateform.Server.Core.Application.DTOs
{
    public class StateDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public Guid CountryId { get; set; }
        public string CountryName { get; set; }
        public bool IsActive { get; set; }
        public List<CityDto> Cities { get; set; }
    }

    public class CreateStateDto
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public Guid CountryId { get; set; }
    }

    public class UpdateStateDto
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public Guid CountryId { get; set; }
        public bool IsActive { get; set; }
    }
} 