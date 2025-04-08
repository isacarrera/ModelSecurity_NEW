using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class RolUser
    {
        public int Id { get; set; }
        public bool Active { get; set; }


        // Claves foráneas
        public int UserId { get; set; } /// FK
        public User User { get; set; } /// PN

        public int RolId { get; set; } /// FK
        public Rol Rol { get; set; } /// PN
    }

}
