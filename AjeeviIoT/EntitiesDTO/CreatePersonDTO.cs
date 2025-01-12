using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AjeeviIoT.EntitiesDTO
{
    public class CreatePersonDTO
    {
        //public int EntityAddressId { get; set; }  // Link to the Entity
        public int? AddressId { get; set; }
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public string? Mobilenumber { get; set; }
        public string? Email { get; set; }
        public List<IFormFile>? Images { get; set; } 
        public int? Contacttype { get; set; }
        public string? Alternatenumber { get; set; } 
        public string? Remarks { get; set; }
    }
}
