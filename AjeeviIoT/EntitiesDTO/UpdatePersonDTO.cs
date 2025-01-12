using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AjeeviIoT.EntitiesDTO
{
    public class UpdatePersonDTO
    {
        //public int? EntityAddressId { get; set; }

        public int? AddressId { get; set; }
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public string? Mobilenumber { get; set; }
        public string? Email { get; set; }
        public int? Contacttype { get; set; }
        public string? Alternatenumber { get; set; }
        public string? Remarks { get; set; }
        public List<IFormFile>? Images { get; set; }
    }
}
