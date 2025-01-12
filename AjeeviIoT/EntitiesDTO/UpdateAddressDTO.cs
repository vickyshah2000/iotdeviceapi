using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AjeeviIoT.EntitiesDTO
{
    public class UpdateAddressDTO
    {
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? Pincode { get; set; }
        public int? CityId { get; set; }
        public int? StateId { get; set; }
        public int? CountryId { get; set; }
        public int? RegionId { get; set; }
        public int? WardId { get; set; }
        public int? ZoneId { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public string? Remarks { get; set; }
    }
}
