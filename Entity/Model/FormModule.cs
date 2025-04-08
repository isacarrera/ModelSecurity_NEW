using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class FormModule
    {
        public int Id { get; set; }
        public bool Active { get; set; }


        /// Claves foraneas
        public int FormId { get; set; } /// FK
        public Form Form { get; set; } /// PN

        public int ModuleId { get; set; } /// FK
        public Module Module { get; set; } /// PN
    }
}
