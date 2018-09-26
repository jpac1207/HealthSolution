using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HealthSolution.Models
{
    [Table("tbpagamento_procedimento")]
    public class PagamentoProcedimento : Pagamento
    {
        public int ProcedimentoId { get; set; }
        public Procedimento Procedimento { get; set; }
    }
}