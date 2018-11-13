namespace HealthSolution.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class booleanprocedimento : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.tbintervencao", "AtendimentoRealizado", c => c.Boolean(nullable: false));
            DropColumn("dbo.tbprocedimento", "AtendimentoRealizado");
        }
        
        public override void Down()
        {
            AddColumn("dbo.tbprocedimento", "AtendimentoRealizado", c => c.Boolean(nullable: false));
            DropColumn("dbo.tbintervencao", "AtendimentoRealizado");
        }
    }
}
