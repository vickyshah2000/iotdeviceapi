using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AjeeviIoT.EntitiesDTO
{
    public class EntityDto
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? ShortName { get; set; }

        public string? LongName { get; set; }

        public string? TradeName { get; set; }

        public string? Remarks { get; set; }

        // Related EntityType and EntityRole names
        public string? EntityTypeName { get; set; }

        public string? EntityRoleName { get; set; }

        public List<ImageDetailDto>? Images { get; set; }

    }
}
