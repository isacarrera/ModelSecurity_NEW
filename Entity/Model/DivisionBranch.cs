using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class DivisionBranch
    {
        public int Id { get; set; }
        public bool Active { get; set; }


        //Clave foranea con Division
        public int DivisionId { get; set; } /// PK
        public Division Division { get; set; } /// PN

        /// Clave foranea con Branch
        public int BranchId { get; set; } /// PK
        public Branch Branch { get; set; } /// PN
    }
}
