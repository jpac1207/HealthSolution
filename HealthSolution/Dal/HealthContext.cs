using HealthSolution.Models;
using MySql.Data.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace HealthSolution.Dal
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class HealthContext : DbContext
    {
        public HealthContext() : base("HealthContext")
        {

        }

        public IDbSet<Paciente> Pacientes { get; set; }
        public IDbSet<Endereco> Enderecos { get; set; }
        public IDbSet<Telefone> Telefones { get; set; }
        public IDbSet<Especialidade> Especialidades { get; set; }
        public IDbSet<Especialista> Especialistas { get; set; }
        public IDbSet<EspecialistaEspecialidade> EspecialistasEspecialidades { get; set; }
        public IDbSet<Consulta> Consultas { get; set; }
        public IDbSet<Procedimento> Procedimentos { get; set; }
        public IDbSet<Intervencao> Intervencoes { get; set; }
        public IDbSet<FormaPagamento> FormasPagamento { get; set; }
        public IDbSet<PagamentoConsulta> PagamentosConsultas { get; set; }
        public IDbSet<PagamentoProcedimento> PagamentosProcedimentos { get; set; }
        public IDbSet<DiasAtendimento> DiasAtendimentos { get; set; }
        public IDbSet<Usuario> Usuarios { get; set; }
        public IDbSet<Arquivo> Arquivos { get; set; }
        public IDbSet<Medicamento> Medicamentos { get; set; }
        public IDbSet<AtendimentoArquivo> AtendimentoArquivo { get; set; }
        public IDbSet<ModeloAnamnese> ModelosAnamneses { get; set; }
        
    }
}