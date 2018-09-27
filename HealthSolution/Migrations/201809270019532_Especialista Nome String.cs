namespace HealthSolution.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EspecialistaNomeString : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.tbespecialista", "Nome", c => c.String(unicode: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.tbespecialista", "Nome", c => c.Int(nullable: false));
        }
    }
}
