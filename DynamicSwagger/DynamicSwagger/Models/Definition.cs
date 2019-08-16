using System.Collections.Generic;

namespace DynamicSwagger.Models
{
    public class ItemDefinition
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<ItemProperty> Properties { get; set; }
    }

    public class ItemProperty
    {
        public string Name { get; set; }
        public string DataType { get; set; }
    }

    public class ItemArrayProperty : ItemProperty
    {
        public List<ItemProperty> Properties { get; set; }
    }
}
