using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class RolFormPermission
    {
        public int Id { get; set; }
        public bool Active { get; set; }


        // Claves foraneas
        public int RolId { get; set; } /// FK 
        public Rol Rol { get; set; } /// PN  

        public int PermissionId { get; set; } /// FK
        public Permission Permission { get; set; } /// PN

        public int FormId { get; set; } /// FK
        public Form Form { get; set; } /// PN 
    }
}
