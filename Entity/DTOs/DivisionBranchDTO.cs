using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity.Model;

namespace Entity.DTOs
{
    public class DivisionBranchDTO
    {
        public int Id { get; set; }
        public bool Status { get; set; }


        public int DivisionId { get; set; }
        public string DivisionName { get; set; }

        public int BranchId { get; set; }
        public string BranchName { get; set; }
    }
}
