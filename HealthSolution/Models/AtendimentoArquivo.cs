using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HealthSolution.Models
{
    [Table("tbatendimento_arquivo")]
    public class AtendimentoArquivo
    {
        public int Id { get; set; }
        public int ArquivoId { get; set; }
        public Arquivo Arquivo { get; set; }
        public int AtendimentoId { get; set; }
        public string Tipo { get; set; }
    }
}