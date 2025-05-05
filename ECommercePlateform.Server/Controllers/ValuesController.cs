using ECommercePlateform.Server.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace ECommercePlateform.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private static readonly string[] Names = new[]
        {
            "India", "China", "USA", "Canada", "Russia"
        };

        private readonly ILogger<ValuesController> _logger;

        public ValuesController(ILogger<ValuesController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetValues")]
        public IEnumerable<Values> Get()
        {
            _logger.LogInformation("Retrieving values");
            return Enumerable.Range(1, 5).Select(index =>
                new Values
                {
                    Id = index,
                    Name = Names[index - 1]
                })
                .ToArray();
        }
    }
}
