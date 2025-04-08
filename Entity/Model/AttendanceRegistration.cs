using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class AttendanceRegistration
    {
        public int Id { get; set; }

        public DateTime Hour { get; set; } 
        public bool IsEntrance  { get; set; }
        public bool Active { get; set; }

        /// Clave foranea con Attendance
        public int AttendanceId { get; set; } /// FK
        public Attendance Attendance { get; set; } /// PN

        public int AccessPointId { get; set; } /// FK
        public AccessPoint AccessPoints { get; set; } /// PN


    }
}
