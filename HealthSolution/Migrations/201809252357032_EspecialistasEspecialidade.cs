namespace HealthSolution.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EspecialistasEspecialidade : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tbespecialista_especialidade",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EspecialistaId = c.Int(nullable: false),
                        EspecialidadeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.tbespecialidade", t => t.EspecialidadeId, cascadeDelete: true)
                .ForeignKey("dbo.tbespecialista", t => t.EspecialistaId, cascadeDelete: true)
                .Index(t => t.EspecialistaId)
                .Index(t => t.EspecialidadeId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.tbespecialista_especialidade", "EspecialistaId", "dbo.tbespecialista");
            DropForeignKey("dbo.tbespecialista_especialidade", "EspecialidadeId", "dbo.tbespecialidade");
            DropIndex("dbo.tbespecialista_especialidade", new[] { "EspecialidadeId" });
            DropIndex("dbo.tbespecialista_especialidade", new[] { "EspecialistaId" });
            DropTable("dbo.tbespecialista_especialidade");
        }
    }
}
