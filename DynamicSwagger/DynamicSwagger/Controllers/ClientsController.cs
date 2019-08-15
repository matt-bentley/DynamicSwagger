using System.Collections.Generic;
using DynamicSwagger.Models;
using Microsoft.AspNetCore.Mvc;

namespace DynamicSwagger.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        // GET api/clients
        [HttpGet]
        public ActionResult<IEnumerable<Client>> Get()
        {
            return new List<Client>();
        }

        // GET api/clients/5
        [HttpGet("{id}")]
        public ActionResult<Client> Get(int id)
        {
            return new Client();
        }

        // POST api/clients
        [HttpPost]
        [ProducesResponseType(201)]
        public void Post([FromBody] Client client)
        {
        }

        // PUT api/clients/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] Client client)
        {
        }

        // DELETE api/clients/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
