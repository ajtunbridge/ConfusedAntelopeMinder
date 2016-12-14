namespace ngen.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InversePropertyOnWorkCentreGroupParent : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.WorkCentreGroups", new[] { "WorkCentreGroup_Id" });
            DropColumn("dbo.WorkCentreGroups", "ParentGroupId");
            RenameColumn(table: "dbo.WorkCentreGroups", name: "WorkCentreGroup_Id", newName: "ParentGroupId");
        }
        
        public override void Down()
        {
            RenameColumn(table: "dbo.WorkCentreGroups", name: "ParentGroupId", newName: "WorkCentreGroup_Id");
            AddColumn("dbo.WorkCentreGroups", "ParentGroupId", c => c.Int());
            CreateIndex("dbo.WorkCentreGroups", "WorkCentreGroup_Id");
        }
    }
}