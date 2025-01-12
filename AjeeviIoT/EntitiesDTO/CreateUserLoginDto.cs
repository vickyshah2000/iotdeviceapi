using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AjeeviIoT.EntitiesDTO
{
    public class CreateUserLoginDto
    {
        public int UserId { get; set; }
        public string UserFname { get; set; }
        public string UserLname { get; set; }
        public string Email { get; set; }
        public string MobileNo { get; set; }
        public int? EntityId { get; set; }
        public string Password { get; set; }
        public bool? Status { get; set; }
        public int? RoleId { get; set; }
        public DateTime? LoginDate { get; set; }
    }

    public class UpdateUserLoginDto
    {
        public string UserFname { get; set; }
        public string UserLname { get; set; }
        public string Email { get; set; }
        public string MobileNo { get; set; }
        public int? EntityId { get; set; }
        public string Password { get; set; }
        public int? RoleId { get; set; }
    }

}
