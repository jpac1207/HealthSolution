namespace HealthSolution.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class campomedicamentos : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.tbconsulta", "Medicamentos", c => c.String(unicode: false));
            AddColumn("dbo.tbintervencao", "Medicamentos", c => c.String(unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.tbintervencao", "Medicamentos");
            DropColumn("dbo.tbconsulta", "Medicamentos");
        }
    }
}
