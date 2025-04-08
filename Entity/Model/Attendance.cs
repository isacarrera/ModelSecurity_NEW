using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class Attendance
    {
        public int Id { get; set; }
        public bool Active { get; set; }


        /// Clave foranea con Card
        public int CardId { get; set; } /// FK
        public Card Card { get; set; } /// PN

        /// Clave foranea con EventSession
        public int EventSessionId { get; set; } /// FK
        public EventSession EventSession { get; set; } /// PN

        // Relaciones
        public ICollection<AttendanceRegistration> Registrations { get; set; } = new List<AttendanceRegistration>(); /// PNI - Relacion uno a muchos
    }
}
