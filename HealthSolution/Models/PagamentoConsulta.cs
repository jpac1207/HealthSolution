using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HealthSolution.Models
{
    [Table("tbpagamento_consulta")]
    public class PagamentoConsulta : Pagamento
    {
        public int ConsultaId { get; set; }
        public Consulta Consulta { get; set; }
    }
}