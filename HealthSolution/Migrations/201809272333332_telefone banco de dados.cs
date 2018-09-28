namespace HealthSolution.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class telefonebancodedados : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("tbtelefone", "PacienteId", "tbpaciente");
            DropIndex("tbtelefone", new[] { "PacienteId" });
            AddColumn("tbespecialista", "TelefoneId", c => c.Int(nullable: false));
            AddColumn("tbpaciente", "TelefoneId", c => c.Int(nullable: false));
            CreateIndex("tbespecialista", "TelefoneId");
            CreateIndex("tbpaciente", "TelefoneId");
            AddForeignKey("tbespecialista", "TelefoneId", "tbtelefone", "Id", cascadeDelete: true);
            AddForeignKey("tbpaciente", "TelefoneId", "tbtelefone", "Id", cascadeDelete: true);
            DropColumn("tbtelefone", "PacienteId");
        }
        
        public override void Down()
        {
            AddColumn("tbtelefone", "PacienteId", c => c.Int(nullable: false));
            DropForeignKey("tbpaciente", "TelefoneId", "tbtelefone");
            DropForeignKey("tbespecialista", "TelefoneId", "tbtelefone");
            DropIndex("tbpaciente", new[] { "TelefoneId" });
            DropIndex("tbespecialista", new[] { "TelefoneId" });
            DropColumn("tbpaciente", "TelefoneId");
            DropColumn("tbespecialista", "TelefoneId");
            CreateIndex("tbtelefone", "PacienteId");
            AddForeignKey("tbtelefone", "PacienteId", "tbpaciente", "Id", cascadeDelete: true);
        }
    }
}
