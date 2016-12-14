namespace ngen.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProductionMethodAndWorkCentreGroupAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.WorkCentreGroups",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        DefaultHourlyRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ParentGroupId = c.Int(),
                        WorkCentreGroup_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.WorkCentreGroups", t => t.WorkCentreGroup_Id)
                .ForeignKey("dbo.WorkCentreGroups", t => t.ParentGroupId)
                .Index(t => t.ParentGroupId)
                .Index(t => t.WorkCentreGroup_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.WorkCentreGroups", "ParentGroupId", "dbo.WorkCentreGroups");
            DropForeignKey("dbo.WorkCentreGroups", "WorkCentreGroup_Id", "dbo.WorkCentreGroups");
            DropIndex("dbo.WorkCentreGroups", new[] { "WorkCentreGroup_Id" });
            DropIndex("dbo.WorkCentreGroups", new[] { "ParentGroupId" });
            DropTable("dbo.WorkCentreGroups");
        }
    }
}
