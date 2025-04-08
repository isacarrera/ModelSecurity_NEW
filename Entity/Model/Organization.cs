using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class Organization
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public bool Active { get; set; }


        //Relaciones
        public List<Branch> Branches { get; set; } = new List<Branch>(); /// PNI - Relacion uno a muchos
    }
}
