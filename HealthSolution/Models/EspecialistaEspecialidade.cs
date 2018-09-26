using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HealthSolution.Models
{
    [Table("tbespecialista_especialidade")]
    public class EspecialistaEspecialidade
    {
        public int Id { get; set; }
        public int EspecialistaId { get; set; }
        public Especialista Especialista { get; set; }
        public int EspecialidadeId { get; set; }
        public Especialidade Especialidade { get; set; }
    }
}