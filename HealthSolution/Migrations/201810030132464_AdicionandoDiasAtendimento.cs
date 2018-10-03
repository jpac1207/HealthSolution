namespace HealthSolution.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AdicionandoDiasAtendimento : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tbdiasatendimento",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AtendeSegunda = c.Boolean(nullable: false),
                        AtendeTerca = c.Boolean(nullable: false),
                        AtendeQuarta = c.Boolean(nullable: false),
                        AtendeQuinta = c.Boolean(nullable: false),
                        AtendeSexta = c.Boolean(nullable: false),
                        AtendeSabado = c.Boolean(nullable: false),
                        AtendeDomingo = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.tbespecialista", "DiasAtendimentoId", c => c.Int(nullable: false));
            CreateIndex("dbo.tbespecialista", "DiasAtendimentoId");
            AddForeignKey("dbo.tbespecialista", "DiasAtendimentoId", "dbo.tbdiasatendimento", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.tbespecialista", "DiasAtendimentoId", "dbo.tbdiasatendimento");
            DropIndex("dbo.tbespecialista", new[] { "DiasAtendimentoId" });
            DropColumn("dbo.tbespecialista", "DiasAtendimentoId");
            DropTable("dbo.tbdiasatendimento");
        }
    }
}
