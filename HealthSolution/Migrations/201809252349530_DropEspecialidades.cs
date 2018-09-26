namespace HealthSolution.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DropEspecialidades : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("tbespecialidade", "Especialista_Id", "tbespecialista");
            DropIndex("tbespecialidade", new[] { "Especialista_Id" });
            DropColumn("tbespecialidade", "Especialista_Id");
        }
        
        public override void Down()
        {
            AddColumn("tbespecialidade", "Especialista_Id", c => c.Int());
            CreateIndex("tbespecialidade", "Especialista_Id");
            AddForeignKey("tbespecialidade", "Especialista_Id", "tbespecialista", "Id");
        }
    }
}
