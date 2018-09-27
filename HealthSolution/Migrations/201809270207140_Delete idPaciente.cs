namespace HealthSolution.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeleteidPaciente : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.tbendereco", "PacienteId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.tbendereco", "PacienteId", c => c.Int(nullable: false));
        }
    }
}
