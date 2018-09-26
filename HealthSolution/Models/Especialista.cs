﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HealthSolution.Models
{
    [Table("tbespecialista")]
    public class Especialista
    {
        public int Id { get; set; }
        public int Nome { get; set; }
        public string Crm { get; set; }       
    }
}