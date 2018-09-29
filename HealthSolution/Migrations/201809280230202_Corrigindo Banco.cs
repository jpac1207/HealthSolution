namespace HealthSolution.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CorrigindoBanco : DbMigration
    {
        public override void Up()
        {
            //DropForeignKey("tbtelefone", "PacienteId", "tbpaciente");
            //DropIndex("tbtelefone", new[] { "PacienteId" });
            //AddColumn("tbespecialista", "TelefoneId", c => c.Int(nullable: false));
            //AddColumn("tbpaciente", "TelefoneId", c => c.Int(nullable: false));
            //CreateIndex("tbespecialista", "TelefoneId");
            //CreateIndex("tbpaciente", "TelefoneId");
            AddForeignKey("tbespecialista", "TelefoneId", "tbtelefone", "Id", cascadeDelete: true);
            AddForeignKey("tbpaciente", "TelefoneId", "tbtelefone", "Id", cascadeDelete: true);
            DropColumn("tbtelefone", "PacienteId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.tbtelefone", "PacienteId", c => c.Int(nullable: false));
            DropForeignKey("dbo.tbpaciente", "TelefoneId", "dbo.tbtelefone");
            DropForeignKey("dbo.tbespecialista", "TelefoneId", "dbo.tbtelefone");
            DropIndex("dbo.tbpaciente", new[] { "TelefoneId" });
            DropIndex("dbo.tbespecialista", new[] { "TelefoneId" });
            DropColumn("dbo.tbpaciente", "TelefoneId");
            DropColumn("dbo.tbespecialista", "TelefoneId");
            CreateIndex("dbo.tbtelefone", "PacienteId");
            AddForeignKey("dbo.tbtelefone", "PacienteId", "dbo.tbpaciente", "Id", cascadeDelete: true);
        }
    }
}
