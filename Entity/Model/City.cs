using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class City
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }


        /// Clave foranea con Deparment
        public int DepartmentId { get; set; } /// FK
        public Department Department { get; set; } /// PN


        ///Relaciones
        public Person Person { get; set; } /// PNI - Relacion uno a uno
    }
}

