using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HealthSolution.Models
{
    [Table("tbmodelo_anamnese")]
    public class ModeloAnamnese
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Modelo { get; set; }
    }
}