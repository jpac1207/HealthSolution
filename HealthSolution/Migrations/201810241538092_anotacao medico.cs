namespace HealthSolution.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class anotacaomedico : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.tbconsulta", "AnotacaoEspecialista", c => c.String(unicode: false));
            AddColumn("dbo.tbintervencao", "AnotacaoEspecialista", c => c.String(unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.tbintervencao", "AnotacaoEspecialista");
            DropColumn("dbo.tbconsulta", "AnotacaoEspecialista");
        }
    }
}
