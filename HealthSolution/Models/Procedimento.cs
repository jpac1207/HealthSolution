using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HealthSolution.Models
{
    [Table("tbprocedimento")]
    public class Procedimento
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public double Valor { get; set; }
    }
}