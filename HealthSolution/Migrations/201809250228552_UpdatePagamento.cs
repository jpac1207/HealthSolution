namespace HealthSolution.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatePagamento : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.tbformapagamento", "Nome", c => c.String(unicode: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.tbformapagamento", "Nome", c => c.Double(nullable: false));
        }
    }
}
