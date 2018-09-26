using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HealthSolution.Models
{
    [Table("tbformapagamento")]
    public class FormaPagamento
    {
        public int Id { get; set; }
        public string Nome { get; set; }
    }
}