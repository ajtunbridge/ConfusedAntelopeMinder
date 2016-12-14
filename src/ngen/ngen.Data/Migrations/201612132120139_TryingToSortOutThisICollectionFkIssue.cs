namespace ngen.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TryingToSortOutThisICollectionFkIssue : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.WorkCentres",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        Caption = c.String(nullable: false, maxLength: 50),
                        WorkCentreGroupId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.WorkCentreGroups", t => t.WorkCentreGroupId, cascadeDelete: true)
                .Index(t => t.Caption, unique: true)
                .Index(t => t.WorkCentreGroupId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.WorkCentres", "WorkCentreGroupId", "dbo.WorkCentreGroups");
            DropIndex("dbo.WorkCentres", new[] { "WorkCentreGroupId" });
            DropIndex("dbo.WorkCentres", new[] { "Caption" });
            DropTable("dbo.WorkCentres");
        }
    }
}
