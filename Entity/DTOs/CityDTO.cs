using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity.Model;

namespace Entity.DTOs
{
    public class CityDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Status { get; set; }


        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
    }
}
