namespace HealthSolution.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class booleanconsultaEAtendimento : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.tbconsulta", "AtendimentoRealizado", c => c.Boolean(nullable: false));
            AddColumn("dbo.tbprocedimento", "AtendimentoRealizado", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.tbprocedimento", "AtendimentoRealizado");
            DropColumn("dbo.tbconsulta", "AtendimentoRealizado");
        }
    }
}
