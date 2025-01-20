using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AjeeviIoT.EntitiesDTO
{
    public class CreateEmployeeDeviceDTO
    {
        public int? PersonId { get; set; }

        public int? DeviceId { get; set; }

        public DateTime? Assigntime { get; set; }

        public ulong? Status { get; set; }
    }
}
