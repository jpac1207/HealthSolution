namespace HealthSolution.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddArquivo : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tbarquivo",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Path = c.String(unicode: false),
                        OriginalName = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.tbconsulta", "ArquivoId", c => c.Int(nullable: false));
            AddColumn("dbo.tbintervencao", "ArquivoId", c => c.Int(nullable: false));
            CreateIndex("dbo.tbconsulta", "ArquivoId");
            CreateIndex("dbo.tbintervencao", "ArquivoId");
            AddForeignKey("dbo.tbconsulta", "ArquivoId", "dbo.tbarquivo", "Id", cascadeDelete: true);
            AddForeignKey("dbo.tbintervencao", "ArquivoId", "dbo.tbarquivo", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.tbintervencao", "ArquivoId", "dbo.tbarquivo");
            DropForeignKey("dbo.tbconsulta", "ArquivoId", "dbo.tbarquivo");
            DropIndex("dbo.tbintervencao", new[] { "ArquivoId" });
            DropIndex("dbo.tbconsulta", new[] { "ArquivoId" });
            DropColumn("dbo.tbintervencao", "ArquivoId");
            DropColumn("dbo.tbconsulta", "ArquivoId");
            DropTable("dbo.tbarquivo");
        }
    }
}
