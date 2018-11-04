namespace HealthSolution.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModeloAnamneses : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tbmodelo_anamnese",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nome = c.String(unicode: false),
                        Modelo = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.tbmodelo_anamnese");
        }
    }
}
