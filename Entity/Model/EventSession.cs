using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Entity.Model
{
    public class EventSession
    {
        public int Id { get; set; }
        public string Name { get; set; } 
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool Active { get; set; }


        //Clave foranea con Event
        public int EventId { get; set; } /// FK
        public Event Event { get; set; } /// PN

        // Relaciones
        public List<Attendance> Attendances { get; set; } = new List<Attendance>();
    }
}
