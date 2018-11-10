using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HealthSolution.Models
{
    [Table("tbusuarios")]
    public class Usuario
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string HashValue { get; set; }
        public bool IsMedical { get; set; }
    }
}