namespace HealthSolution.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MudançasTabelaEndereços : DbMigration
    {
        public override void Up()
        {
            //DropForeignKey("tbendereco", "PacienteId", "tbpaciente");
            //DropIndex("tbendereco", new[] { "PacienteId" });
            //AddColumn("tbespecialista", "ConselhoUF", c => c.String(unicode: false));
            //AddColumn("tbespecialista", "DataNascimento", c => c.DateTime(nullable: false, precision: 0));
            //AddColumn("tbespecialista", "Email", c => c.String(unicode: false));
            //AddColumn("tbpaciente", "EnderecoId", c => c.Int(nullable: false));
            //CreateIndex("tbpaciente", "EnderecoId");
            //AddForeignKey("tbpaciente", "EnderecoId", "tbendereco", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.tbpaciente", "EnderecoId", "dbo.tbendereco");
            DropIndex("dbo.tbpaciente", new[] { "EnderecoId" });
            DropColumn("dbo.tbpaciente", "EnderecoId");
            DropColumn("dbo.tbespecialista", "Email");
            DropColumn("dbo.tbespecialista", "DataNascimento");
            DropColumn("dbo.tbespecialista", "ConselhoUF");
            CreateIndex("dbo.tbendereco", "PacienteId");
            AddForeignKey("dbo.tbendereco", "PacienteId", "dbo.tbpaciente", "Id", cascadeDelete: true);
        }
    }
}
