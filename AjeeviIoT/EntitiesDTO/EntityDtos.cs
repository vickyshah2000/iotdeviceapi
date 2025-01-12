using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AjeeviIoT.EntitiesDTO
{
    public class EntityDtos
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? ShortName { get; set; }
        public string? LongName { get; set; }
        public string? TradeName { get; set; }
        public string? Remarks { get; set; }
        public string? EntityTypeName { get; set; }
        public string? EntityRoleName { get; set; }
        public List<ImageDetailDto>? Images { get; set; }
    }

    public class ImageDetailDto
    {
        public int ImageId { get; set; }
        public string? ImageUrl { get; set; }
        public string? UniqueId { get; set; }
    }

}
