namespace HealthSolution.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class HorarioAtendimento : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.tbespecialista", "HoraInicial", c => c.Int(nullable: false));
            AddColumn("dbo.tbespecialista", "MinutoInicial", c => c.Int(nullable: false));
            AddColumn("dbo.tbespecialista", "HoraFinal", c => c.Int(nullable: false));
            AddColumn("dbo.tbespecialista", "MinutoFinal", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.tbespecialista", "MinutoFinal");
            DropColumn("dbo.tbespecialista", "HoraFinal");
            DropColumn("dbo.tbespecialista", "MinutoInicial");
            DropColumn("dbo.tbespecialista", "HoraInicial");
        }
    }
}
