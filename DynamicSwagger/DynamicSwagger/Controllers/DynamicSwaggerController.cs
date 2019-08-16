using DynamicSwagger.Helpers;
using DynamicSwagger.Repositories;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DynamicSwagger.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DynamicSwaggerController : Controller
    {
        private readonly IItemDefinitionsRepository _repo;

        public DynamicSwaggerController(IItemDefinitionsRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            var definition = _repo.Get(id);
            var generator = new SwaggerGenerator(definition);
            var jsonString = generator.Generate();

            //var jsonString = System.IO.File.ReadAllText("dynamic_swagger.json");
            var json = JsonConvert.DeserializeObject(jsonString);
            return Json(json);
        }
    }
}
