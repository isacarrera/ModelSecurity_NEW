using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class Assignment
    {
     
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }


        /// Clave foranea con Division
        public int DivisionId { get; set; } /// FK
        public Division Division { get; set; } /// PN  

        /// Relaciones 
        public List<Person> Persons { get; set; } = new(); /// PNI - Relaciones de uno a muchos

    }
}
