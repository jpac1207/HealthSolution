namespace HealthSolution.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addenderecoIdespecialistas : DbMigration
    {
        public override void Up()
        {
            //AddColumn("tbespecialista", "EnderecoId", c => c.Int(nullable: false));
            //CreateIndex("tbespecialista", "EnderecoId");
            //AddForeignKey("tbespecialista", "EnderecoId", "tbendereco", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            AddColumn("dbo.tbespecialista", "idEndereco", c => c.Int(nullable: false));
            DropForeignKey("dbo.tbespecialista", "EnderecoId", "dbo.tbendereco");
            DropIndex("dbo.tbespecialista", new[] { "EnderecoId" });
            AlterColumn("dbo.tbespecialista", "EnderecoId", c => c.Int());
            RenameColumn(table: "dbo.tbespecialista", name: "EnderecoId", newName: "endereco_Id");
            CreateIndex("dbo.tbespecialista", "endereco_Id");
            AddForeignKey("dbo.tbespecialista", "endereco_Id", "dbo.tbendereco", "Id");
        }
    }
}
