using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DynamicSwagger.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DynamicSwaggerController : Controller
    {
        [HttpGet("{listName}")]
        public IActionResult Get(string listName)
        {
            var jsonString = System.IO.File.ReadAllText("dynamic_swagger.json");
            var json = JsonConvert.DeserializeObject(jsonString);
            return Json(json);
        }
    }
}
