using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HealthSolution.Models
{    
    public abstract class Pagamento
    {
        public int Id { get; set; }
        public int FormaPagamentoId { get; set; }
        public FormaPagamento FormaPagamento { get; set; }
    }
}