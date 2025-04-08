using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool Active { get; set; }

        // Clave foránea con Person
        public int PersonId { get; set; } /// FK
        public Person Person { get; set; } /// PN

        // Relaciones
        public List<RolUser> RolUsers { get; set; } /// PNI - Relacion uno a muchos 
    }
}
