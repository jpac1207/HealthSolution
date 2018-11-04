namespace HealthSolution.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AtendimentoArquivo : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tbatendimento_arquivo",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ArquivoId = c.Int(nullable: false),
                        AtendimentoId = c.Int(nullable: false),
                        Tipo = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.tbarquivo", t => t.ArquivoId, cascadeDelete: true)
                .Index(t => t.ArquivoId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.tbatendimento_arquivo", "ArquivoId", "dbo.tbarquivo");
            DropIndex("dbo.tbatendimento_arquivo", new[] { "ArquivoId" });
            DropTable("dbo.tbatendimento_arquivo");
        }
    }
}
