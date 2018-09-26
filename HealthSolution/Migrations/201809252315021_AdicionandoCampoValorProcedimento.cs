namespace HealthSolution.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AdicionandoCampoValorProcedimento : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.tbprocedimento", "Valor", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.tbprocedimento", "Valor");
        }
    }
}
