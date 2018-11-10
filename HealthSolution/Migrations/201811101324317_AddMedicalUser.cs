namespace HealthSolution.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMedicalUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.tbusuarios", "IsMedical", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.tbusuarios", "IsMedical");
        }
    }
}
