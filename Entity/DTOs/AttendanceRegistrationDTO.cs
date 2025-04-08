using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOs
{
    public class AttendanceRegistrationDTO
    {
        public int Id { get; set; }
        public DateTime Hour { get; set; }
        public bool TypeAccess { get; set; }
        public bool Status { get; set; }


        public int AttendanceId { get; set; }
        public string AttendanceName { get; set; }

        public int AccessPointId { get; set; }
        public string AccessPoinName { get; set; }
    }
}
