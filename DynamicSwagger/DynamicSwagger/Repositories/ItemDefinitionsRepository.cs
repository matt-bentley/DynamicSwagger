using DynamicSwagger.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamicSwagger.Repositories
{
    public interface IItemDefinitionsRepository
    {
        ItemDefinition Get(string id);
    }

    public class ItemDefinitionsRepository : IItemDefinitionsRepository
    {
        private Dictionary<string, ItemDefinition> _definitions;

        public ItemDefinitionsRepository()
        {
            _definitions = new Dictionary<string, ItemDefinition>();
            var clientsDefininion = GetClientsDefinition();
            _definitions.Add(clientsDefininion.Id, clientsDefininion);
        }

        public ItemDefinition Get(string id)
        {
            return _definitions[id];
        }

        private ItemDefinition GetClientsDefinition()
        {
            var definition = new ItemDefinition()
            {
                Id = "5890715b8647a3321ba6b6bb",
                Name = "clients",
                Properties = new List<ItemProperty>()
            };

            definition.Properties.Add(new ItemProperty()
            {
                Name = "id",
                DataType = "string"
            });
            definition.Properties.Add(new ItemProperty()
            {
                Name = "name",
                DataType = "string"
            });
            definition.Properties.Add(new ItemProperty()
            {
                Name = "createdDate",
                DataType = "datetime"
            });
            definition.Properties.Add(GetEntitiesProperties());
            return definition;
        }

        private ItemArrayProperty GetEntitiesProperties()
        {
            var property = new ItemArrayProperty()
            {
                Name = "entities",
                DataType = "array",
                Properties = new List<ItemProperty>()
            };

            property.Properties.Add(new ItemProperty()
            {
                Name = "id",
                DataType = "string"
            });
            property.Properties.Add(new ItemProperty()
            {
                Name = "name",
                DataType = "string"
            });
            property.Properties.Add(GetDirectorsProperties());
            return property;
        }

        private ItemArrayProperty GetDirectorsProperties()
        {
            var property = new ItemArrayProperty()
            {
                Name = "directors",
                DataType = "array",
                Properties = new List<ItemProperty>()
            };

            property.Properties.Add(new ItemProperty()
            {
                Name = "id",
                DataType = "string"
            });
            property.Properties.Add(new ItemProperty()
            {
                Name = "firstName",
                DataType = "string"
            });
            property.Properties.Add(new ItemProperty()
            {
                Name = "lastName",
                DataType = "string"
            });
            property.Properties.Add(new ItemProperty()
            {
                Name = "age",
                DataType = "integer"
            });
            return property;
        }
    }
}
