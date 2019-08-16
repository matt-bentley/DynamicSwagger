using System;
using System.Collections.Generic;

namespace DynamicSwagger.Models
{
    public class Client
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<Entity> Entities { get; set; }
    }

    public class Entity
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
