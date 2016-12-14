namespace ngen.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedOperationEntity : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Operations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Sequence = c.Byte(nullable: false),
                        Description = c.String(),
                        SetupTime = c.Double(nullable: false),
                        CycleTime = c.Double(nullable: false),
                        ProductionMethodId = c.Int(nullable: false),
                        WorkCentreId = c.Int(),
                        WorkCentreGroupId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ProductionMethods", t => t.ProductionMethodId, cascadeDelete: true)
                .ForeignKey("dbo.WorkCentres", t => t.WorkCentreId)
                .ForeignKey("dbo.WorkCentreGroups", t => t.WorkCentreGroupId)
                .Index(t => t.ProductionMethodId)
                .Index(t => t.WorkCentreId)
                .Index(t => t.WorkCentreGroupId);
            
            CreateTable(
                "dbo.ProductionMethods",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        Rating = c.Byte(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        IsProven = c.Boolean(nullable: false),
                        CreatedById = c.Int(nullable: false),
                        PartVersionId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Employees", t => t.CreatedById, cascadeDelete: true)
                .ForeignKey("dbo.PartVersions", t => t.PartVersionId, cascadeDelete: true)
                .Index(t => t.CreatedById)
                .Index(t => t.PartVersionId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Operations", "WorkCentreGroupId", "dbo.WorkCentreGroups");
            DropForeignKey("dbo.Operations", "WorkCentreId", "dbo.WorkCentres");
            DropForeignKey("dbo.ProductionMethods", "PartVersionId", "dbo.PartVersions");
            DropForeignKey("dbo.Operations", "ProductionMethodId", "dbo.ProductionMethods");
            DropForeignKey("dbo.ProductionMethods", "CreatedById", "dbo.Employees");
            DropIndex("dbo.ProductionMethods", new[] { "PartVersionId" });
            DropIndex("dbo.ProductionMethods", new[] { "CreatedById" });
            DropIndex("dbo.Operations", new[] { "WorkCentreGroupId" });
            DropIndex("dbo.Operations", new[] { "WorkCentreId" });
            DropIndex("dbo.Operations", new[] { "ProductionMethodId" });
            DropTable("dbo.ProductionMethods");
            DropTable("dbo.Operations");
        }
    }
}
