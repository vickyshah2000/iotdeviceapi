using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AjeeviIoT.EntitiesDTO
{
    public class CreateEntityRelationshipDTO
    {
        public int EntityId { get; set; }
        public int? EntityRoleId { get; set; }
        public string? EntityRoleName { get; set; }
        public string? Name { get; set; }
        public string? ShortName { get; set; }
        public string? LongName { get; set; }
        public string? TradeName { get; set; }
        public string? Remarks { get; set; }
        public string? EntityTypeName { get; set; }
        public string? RoleName { get; set; }
    }
}
