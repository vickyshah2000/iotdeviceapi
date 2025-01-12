using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AjeeviIoT.EntitiesDTO
{
    public class EntityRelationshipDTO
    {
        public int EntityId { get; set; }
        public int? RelationshipId { get; set; }
        public int? EntityRoleId { get; set; }
        public string? RoleName { get; set; }
        public string? Name { get; set; }
        public string? ShortName { get; set; }
        public string? LongName { get; set; }
        public string? TradeName { get; set; }
        public string? EntityTypeName { get; set; }
        public string? Remarks { get; set; }
    }
}
