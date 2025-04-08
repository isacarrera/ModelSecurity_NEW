﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class Person
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string DocumentNumber { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string DocumentType { get; set; }
        public string BloodType { get; set; }
        public bool Active { get; set; }


        /// Clave foranea con City
        //public int CityId { get; set; } /// FK
        //public City City { get; set; } /// PN

        /// Clave foranea con Assignament
        //public int AssignamentId {  get; set; } // FK
        //public Assignment Assignment { get; set; } // PN

        /// Relaciones
        public User User { get; set; }  /// PNI - Relacion uno a uno
        //public List<User> Users { get; set; } = [];

    }
}
