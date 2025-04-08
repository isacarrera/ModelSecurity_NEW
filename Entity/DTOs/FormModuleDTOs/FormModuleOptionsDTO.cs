using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOs.FormModuleDTOs
{
    public class FormModuleOptionsDTO
    {
        public int Id { get; set; }
        public bool Status { get; set; }

        public int FormId { get; set; }
        public int ModuleId { get; set; }
    }
}
