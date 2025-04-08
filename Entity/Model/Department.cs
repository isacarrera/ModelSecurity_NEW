using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{

    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }


        /// Relaciones
        public List<City> Cities { get; set; } = new List<City>(); /// PNI - Relacion uno a muchos
    }

}
