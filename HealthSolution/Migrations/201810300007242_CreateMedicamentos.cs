namespace HealthSolution.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateMedicamentos : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Medicamentos",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nome = c.String(unicode: false),
                        Apresentacao = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Medicamentos");
        }
    }
}
