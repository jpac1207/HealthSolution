using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HealthSolution.Models
{
    [Table("tbdiasatendimento")]
    public class DiasAtendimento
    {
        public int Id { get; set; }
        public bool AtendeSegunda { get; set; }
        public bool AtendeTerca { get; set; }
        public bool AtendeQuarta { get; set; }
        public bool AtendeQuinta { get; set; }
        public bool AtendeSexta { get; set; }
        public bool AtendeSabado { get; set; }
        public bool AtendeDomingo { get; set; }
    }
}