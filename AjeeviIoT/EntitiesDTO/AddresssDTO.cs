using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AjeeviIoT.EntitiesDTO
{
    public class AddresssDTO
    {
        public int EntityId { get; set; }
        public int? AddressId { get; set; }
        public string Addressline1 { get; set; }
        public string Addressline2 { get; set; }
        public string Pincode { get; set; }
        public int? CityId { get; set; }
        public string Cityname { get; set; }
        public int? StateId { get; set; }
        public string Statename { get; set; }
        public int? WardId { get; set; }
        public string Wardname { get; set; }
        public int? RegionId { get; set; }
        public string Regionname { get; set; }
        public int? ZoneId { get; set; }
        public string Zonename { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
        public string CountryName { get; set; }
        public string Remarks { get; set; }
        public int? AddressTypeId { get; set; }
        public string AddressTypeName { get; set; }
    }
}
