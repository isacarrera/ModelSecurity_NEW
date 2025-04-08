using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOs
{
    public class AssignmentDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Status { get; set; }


        public int DivisionId { get; set; }
        public string DivisionName { get; set; }
    }
}
