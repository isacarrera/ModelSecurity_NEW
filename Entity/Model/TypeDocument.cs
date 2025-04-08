using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class TypeDocument
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public bool Active {  get; set; }

        /// Relaciones
        public List<Person> Persons { get; set; } = new List<Person>();
    }
}
