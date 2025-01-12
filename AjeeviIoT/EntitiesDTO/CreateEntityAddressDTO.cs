using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AjeeviIoT.EntitiesDTO
{
    public class CreateEntityAddressDTO
    {
        public int? EntityId { get; set; }
        public List<AddressDTO>? Addresses { get; set; }

        //public string Addressline1 { get; set; }
        //public string Addressline2 { get; set; }
        //public string Pincode { get; set; }
        //public int? CityId { get; set; }
        //public int? StateId { get; set; }
        //public int? WardId { get; set; }
        //public int? RegionId { get; set; }
        //public int? ZoneId { get; set; }
        //public string Latitude { get; set; }
        //public string Longitude { get; set; }
        //public int? CountryId { get; set; }
        //public string Remarks { get; set; }
        //public int AddressType { get; set; }
    }

    public class AddressDTO
    {
        public string? Addressline1 { get; set; }
        public string? Addressline2 { get; set; }
        public string? Pincode { get; set; }
        public int? CityId { get; set; }
        public int? StateId { get; set; }
        public int? WardId { get; set; }
        public int? RegionId { get; set; }
        public int? ZoneId { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public int? CountryId { get; set; }
        public string? Remarks { get; set; }
        public int AddressType { get; set; }
    }
}
