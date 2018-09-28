using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HealthSolution.Models
{
    [Table("tbtelefone")]
    public class Telefone
    {
        public int Id { get; set; }
        public string Numero { get; set; }
    }
}