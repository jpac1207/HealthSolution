namespace HealthSolution.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AdicionaValorPagoAndHoraMinuto : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.tbconsulta", "Hora", c => c.Int(nullable: false));
            AddColumn("dbo.tbconsulta", "Minuto", c => c.Int(nullable: false));
            AddColumn("dbo.tbconsulta", "ValorPago", c => c.Double(nullable: false));
            AddColumn("dbo.tbintervencao", "Hora", c => c.Int(nullable: false));
            AddColumn("dbo.tbintervencao", "Minuto", c => c.Int(nullable: false));
            AddColumn("dbo.tbintervencao", "ValorPago", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.tbintervencao", "ValorPago");
            DropColumn("dbo.tbintervencao", "Minuto");
            DropColumn("dbo.tbintervencao", "Hora");
            DropColumn("dbo.tbconsulta", "ValorPago");
            DropColumn("dbo.tbconsulta", "Minuto");
            DropColumn("dbo.tbconsulta", "Hora");
        }
    }
}
