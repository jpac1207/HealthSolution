using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HealthSolution.Models
{
    [Table("Medicamentos")]
    public class Medicamento
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Apresentacao { get; set; }
    }
}