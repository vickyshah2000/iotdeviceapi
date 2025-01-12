using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AjeeviIoT.EntitiesDTO
{
    public class UpdateEntityDTO
    {
        public string? Name { get; set; }
        public string? ShortName { get; set; }
        public string? LongName { get; set; }
        public string? TradeName { get; set; }
        public int? EntityTypeId { get; set; }
        public int? EntityRoleId { get; set; }
        public string? Remarks { get; set; }
        public List<IFormFile>? Images { get; set; }
    }
}
