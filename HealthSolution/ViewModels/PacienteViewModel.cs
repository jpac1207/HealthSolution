using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HealthSolution.ViewModels
{
    public class PacienteViewModel
    {
        //just to use the framework with scafolding
        public int Id { get; set;}

        public string Nome { get; set; }
        public string Cpf { get; set; }
        public DateTime DataNascimento { get; set; }
        public DateTime DataCadastro { get; set; }
        public string ComoConheceu { get; set; }

        public string Cidade { get; set; }
        public string Bairro { get; set; }
        public string Rua { get; set; }
        public string NumeroResidencia { get; set; }

        public string NumeroTelefone { get; set; }
               
    }
}