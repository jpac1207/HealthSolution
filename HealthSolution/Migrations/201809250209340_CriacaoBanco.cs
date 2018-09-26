namespace HealthSolution.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CriacaoBanco : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tbconsulta",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false, precision: 0),
                        EspecialidadeId = c.Int(nullable: false),
                        EspecialistaId = c.Int(nullable: false),
                        PacienteId = c.Int(nullable: false),
                        Observacao = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.tbespecialidade", t => t.EspecialidadeId, cascadeDelete: true)
                .ForeignKey("dbo.tbespecialista", t => t.EspecialistaId, cascadeDelete: true)
                .ForeignKey("dbo.tbpaciente", t => t.PacienteId, cascadeDelete: true)
                .Index(t => t.EspecialidadeId)
                .Index(t => t.EspecialistaId)
                .Index(t => t.PacienteId);
            
            CreateTable(
                "dbo.tbespecialidade",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nome = c.String(unicode: false),
                        ValorConsulta = c.Double(nullable: false),
                        Especialista_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.tbespecialista", t => t.Especialista_Id)
                .Index(t => t.Especialista_Id);
            
            CreateTable(
                "dbo.tbespecialista",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nome = c.Int(nullable: false),
                        Crm = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.tbpaciente",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nome = c.String(unicode: false),
                        Cpf = c.String(unicode: false),
                        DataNascimento = c.DateTime(nullable: false, precision: 0),
                        DataCadastro = c.DateTime(nullable: false, precision: 0),
                        ComoConheceu = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.tbendereco",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PacienteId = c.Int(nullable: false),
                        Cidade = c.String(unicode: false),
                        Bairro = c.String(unicode: false),
                        Rua = c.String(unicode: false),
                        Numero = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.tbpaciente", t => t.PacienteId, cascadeDelete: true)
                .Index(t => t.PacienteId);
            
            CreateTable(
                "dbo.tbformapagamento",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nome = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.tbintervencao",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false, precision: 0),
                        ProcedimentoId = c.Int(nullable: false),
                        EspecialistaId = c.Int(nullable: false),
                        PacienteId = c.Int(nullable: false),
                        Observacao = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.tbespecialista", t => t.EspecialistaId, cascadeDelete: true)
                .ForeignKey("dbo.tbpaciente", t => t.PacienteId, cascadeDelete: true)
                .ForeignKey("dbo.tbprocedimento", t => t.ProcedimentoId, cascadeDelete: true)
                .Index(t => t.ProcedimentoId)
                .Index(t => t.EspecialistaId)
                .Index(t => t.PacienteId);
            
            CreateTable(
                "dbo.tbprocedimento",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nome = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.tbpagamento_consulta",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ConsultaId = c.Int(nullable: false),
                        FormaPagamentoId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.tbconsulta", t => t.ConsultaId, cascadeDelete: true)
                .ForeignKey("dbo.tbformapagamento", t => t.FormaPagamentoId, cascadeDelete: true)
                .Index(t => t.ConsultaId)
                .Index(t => t.FormaPagamentoId);
            
            CreateTable(
                "dbo.tbpagamento_procedimento",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProcedimentoId = c.Int(nullable: false),
                        FormaPagamentoId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.tbformapagamento", t => t.FormaPagamentoId, cascadeDelete: true)
                .ForeignKey("dbo.tbprocedimento", t => t.ProcedimentoId, cascadeDelete: true)
                .Index(t => t.ProcedimentoId)
                .Index(t => t.FormaPagamentoId);
            
            CreateTable(
                "dbo.tbtelefone",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Numero = c.String(unicode: false),
                        PacienteId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.tbpaciente", t => t.PacienteId, cascadeDelete: true)
                .Index(t => t.PacienteId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.tbtelefone", "PacienteId", "dbo.tbpaciente");
            DropForeignKey("dbo.tbpagamento_procedimento", "ProcedimentoId", "dbo.tbprocedimento");
            DropForeignKey("dbo.tbpagamento_procedimento", "FormaPagamentoId", "dbo.tbformapagamento");
            DropForeignKey("dbo.tbpagamento_consulta", "FormaPagamentoId", "dbo.tbformapagamento");
            DropForeignKey("dbo.tbpagamento_consulta", "ConsultaId", "dbo.tbconsulta");
            DropForeignKey("dbo.tbintervencao", "ProcedimentoId", "dbo.tbprocedimento");
            DropForeignKey("dbo.tbintervencao", "PacienteId", "dbo.tbpaciente");
            DropForeignKey("dbo.tbintervencao", "EspecialistaId", "dbo.tbespecialista");
            DropForeignKey("dbo.tbendereco", "PacienteId", "dbo.tbpaciente");
            DropForeignKey("dbo.tbconsulta", "PacienteId", "dbo.tbpaciente");
            DropForeignKey("dbo.tbconsulta", "EspecialistaId", "dbo.tbespecialista");
            DropForeignKey("dbo.tbespecialidade", "Especialista_Id", "dbo.tbespecialista");
            DropForeignKey("dbo.tbconsulta", "EspecialidadeId", "dbo.tbespecialidade");
            DropIndex("dbo.tbtelefone", new[] { "PacienteId" });
            DropIndex("dbo.tbpagamento_procedimento", new[] { "FormaPagamentoId" });
            DropIndex("dbo.tbpagamento_procedimento", new[] { "ProcedimentoId" });
            DropIndex("dbo.tbpagamento_consulta", new[] { "FormaPagamentoId" });
            DropIndex("dbo.tbpagamento_consulta", new[] { "ConsultaId" });
            DropIndex("dbo.tbintervencao", new[] { "PacienteId" });
            DropIndex("dbo.tbintervencao", new[] { "EspecialistaId" });
            DropIndex("dbo.tbintervencao", new[] { "ProcedimentoId" });
            DropIndex("dbo.tbendereco", new[] { "PacienteId" });
            DropIndex("dbo.tbespecialidade", new[] { "Especialista_Id" });
            DropIndex("dbo.tbconsulta", new[] { "PacienteId" });
            DropIndex("dbo.tbconsulta", new[] { "EspecialistaId" });
            DropIndex("dbo.tbconsulta", new[] { "EspecialidadeId" });
            DropTable("dbo.tbtelefone");
            DropTable("dbo.tbpagamento_procedimento");
            DropTable("dbo.tbpagamento_consulta");
            DropTable("dbo.tbprocedimento");
            DropTable("dbo.tbintervencao");
            DropTable("dbo.tbformapagamento");
            DropTable("dbo.tbendereco");
            DropTable("dbo.tbpaciente");
            DropTable("dbo.tbespecialista");
            DropTable("dbo.tbespecialidade");
            DropTable("dbo.tbconsulta");
        }
    }
}
