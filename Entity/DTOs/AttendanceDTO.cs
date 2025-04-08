using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOs
{
    public class AttendanceDTO
    {
        public int Id { get; set; }
        public bool Status { get; set; }


        public int CardId { get; set; }
        public string CardName { get; set;  }

        public int EventSessionId { get; set; }
        public string EventSessionName { get; set; }
    }
}
