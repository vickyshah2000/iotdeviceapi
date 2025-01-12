using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AjeeviIoT.EntitiesDTO
{
    public class AddressDto
    {
        public int Id { get; set; }
        public string? Addressline1 { get; set; }
        public string? Addressline2 { get; set; }
        public string? Pincode { get; set; }
        public string? CityName { get; set; }
        public string? StateName { get; set; }
        public string? CountryName { get; set; }
        public string? RegionName { get; set; }
        public string? WardName { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public string? Remarks { get; set; }
    }
}
