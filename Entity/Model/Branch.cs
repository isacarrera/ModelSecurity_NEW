using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class Branch
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public bool Active { get; set; }


        //Clave foranera con Organization
        public int OrganizationId { get; set; }
        public Organization Organization { get; set; }

        //Relaciones
        public List<DivisionBranch> DivisionBranches { get; set; } = new List<DivisionBranch>(); /// PNI - Relacion de uno a muchos
    }
}
