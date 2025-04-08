using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOs.RolUserDTOs
{
    public class RolUserOptionsDTO
    {
        public int Id { get; set; }
        public bool Status { get; set; }

        public int RolId { get; set; }

        public int UserId { get; set; }
    }
}
