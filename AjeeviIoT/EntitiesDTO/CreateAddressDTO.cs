using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AjeeviIoT.EntitiesDTO
{
    public class CreateAddressDTO
    {
        public string? Addressline1 { get; set; }
        public string? Addressline2 { get; set; }
        public string? Pincode { get; set; }
        public int? CityId { get; set; }
        public int? Stateid { get; set; }
        public int? CountryId { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public string? Remarks { get; set; }
    }
}
