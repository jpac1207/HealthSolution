namespace HealthSolution.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CorrigindoPagamentoIntervencao : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("tbpagamento_procedimento", "ProcedimentoId", "tbprocedimento");
            DropIndex("tbpagamento_procedimento", new[] { "ProcedimentoId" });
            AddColumn("tbpagamento_procedimento", "IntervencaoId", c => c.Int(nullable: false));
            CreateIndex("tbpagamento_procedimento", "IntervencaoId");
            AddForeignKey("tbpagamento_procedimento", "IntervencaoId", "tbintervencao", "Id", cascadeDelete: true);
            DropColumn("tbpagamento_procedimento", "ProcedimentoId");
        }
        
        public override void Down()
        {
            AddColumn("tbpagamento_procedimento", "ProcedimentoId", c => c.Int(nullable: false));
            DropForeignKey("tbpagamento_procedimento", "IntervencaoId", "tbintervencao");
            DropIndex("tbpagamento_procedimento", new[] { "IntervencaoId" });
            DropColumn("tbpagamento_procedimento", "IntervencaoId");
            CreateIndex("tbpagamento_procedimento", "ProcedimentoId");
            AddForeignKey("tbpagamento_procedimento", "ProcedimentoId", "tbprocedimento", "Id", cascadeDelete: true);
        }
    }
}
