using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOs.RolUserDTOs
{
    public class RolUserDTO
    {
        public int Id { get; set; }
        public bool Status { get; set; }


        public int RolId { get; set; }
        public string RolName { get; set; }

        public int UserId { get; set; }
        public string UserName { get; set; }
    }
}
