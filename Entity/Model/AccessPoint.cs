using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Entity.Model
{
    public class AccessPoint
    {
        public int Id { get; set; }

        public string Name { get; set; } 
        public string Ubication { get; set; }
        public bool Active { get; set; }


        /// Clave Foranea con Event
        public int EventId { get; set; } // FK
        public Event Event { get; set; } // PN

        /// Relaciones
        public List<AttendanceRegistration> AttendanceRegistrations { get; set; } = new List<AttendanceRegistration>(); /// PNI - Relacion uno a muchos
    }
}
