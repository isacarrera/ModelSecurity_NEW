using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class Event
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public DateOnly Date { get; set; }
        public bool Active { get; set; }


        ///Clave foranera con EventType
        public int EventTypeId { get; set; } /// FK
        public EventType EventType { get; set; } /// PN

        ///Relaciones
        public List<EventSession> Sessions { get; set; } = new List<EventSession>(); /// PNI - Relacion uno a muchos
        public List<AccessPoint> AccessPoints { get; set; } = new List<AccessPoint>(); /// PNI - Relacion uno a muhcos
    }
}
