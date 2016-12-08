namespace ngen.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Customers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FullName = c.String(nullable: false, maxLength: 100),
                        ShortName = c.String(maxLength: 40),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.FullName, unique: true)
                .Index(t => t.ShortName, unique: true);
            
            CreateTable(
                "dbo.Parts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DrawingNumber = c.String(nullable: false, maxLength: 50),
                        Name = c.String(nullable: false, maxLength: 100),
                        Primary2dDrawingDocumentId = c.Int(),
                        Primary3dDrawingDocumentId = c.Int(),
                        Customer_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Customers", t => t.Customer_Id)
                .ForeignKey("dbo.Documents", t => t.Primary2dDrawingDocumentId)
                .ForeignKey("dbo.Documents", t => t.Primary3dDrawingDocumentId)
                .Index(t => t.DrawingNumber, unique: true)
                .Index(t => t.Primary2dDrawingDocumentId)
                .Index(t => t.Primary3dDrawingDocumentId)
                .Index(t => t.Customer_Id);
            
            CreateTable(
                "dbo.Documents",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FileName = c.String(nullable: false, maxLength: 100),
                        IsCheckedOut = c.Boolean(nullable: false),
                        Permissions = c.Binary(),
                        CheckedOutAt = c.DateTime(),
                        CheckedOutById = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Employees", t => t.CheckedOutById)
                .Index(t => t.CheckedOutById);
            
            CreateTable(
                "dbo.Employees",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserName = c.String(nullable: false, maxLength: 30),
                        PasswordHash = c.String(nullable: false, maxLength: 88, fixedLength: true),
                        PasswordSalt = c.String(nullable: false, maxLength: 24, fixedLength: true),
                        IsActive = c.Boolean(nullable: false),
                        PersonId = c.Int(nullable: false),
                        SystemRoleId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.People", t => t.PersonId, cascadeDelete: true)
                .ForeignKey("dbo.SystemRoles", t => t.SystemRoleId, cascadeDelete: true)
                .Index(t => t.UserName, unique: true)
                .Index(t => t.PersonId)
                .Index(t => t.SystemRoleId);
            
            CreateTable(
                "dbo.People",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        FirstName = c.String(nullable: false, maxLength: 30),
                        MiddleName = c.String(),
                        LastName = c.String(nullable: false, maxLength: 30),
                        DateOfBirth = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SystemRoles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 30),
                        Description = c.String(),
                        Permissions = c.Binary(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DocumentVersions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Hash = c.String(nullable: false, maxLength: 32, fixedLength: true),
                        Changes = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                        CreatedById = c.Int(nullable: false),
                        DocumentId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Employees", t => t.CreatedById, cascadeDelete: true)
                .ForeignKey("dbo.Documents", t => t.DocumentId, cascadeDelete: true)
                .Index(t => t.CreatedById)
                .Index(t => t.DocumentId);
            
            CreateTable(
                "dbo.PartVersions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        VersionNumber = c.String(nullable: false, maxLength: 12),
                        Changes = c.String(),
                        PartId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Parts", t => t.PartId, cascadeDelete: true)
                .Index(t => t.PartId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PartVersions", "PartId", "dbo.Parts");
            DropForeignKey("dbo.DocumentVersions", "DocumentId", "dbo.Documents");
            DropForeignKey("dbo.DocumentVersions", "CreatedById", "dbo.Employees");
            DropForeignKey("dbo.Parts", "Primary3dDrawingDocumentId", "dbo.Documents");
            DropForeignKey("dbo.Parts", "Primary2dDrawingDocumentId", "dbo.Documents");
            DropForeignKey("dbo.Documents", "CheckedOutById", "dbo.Employees");
            DropForeignKey("dbo.Employees", "SystemRoleId", "dbo.SystemRoles");
            DropForeignKey("dbo.Employees", "PersonId", "dbo.People");
            DropForeignKey("dbo.Parts", "Customer_Id", "dbo.Customers");
            DropIndex("dbo.PartVersions", new[] { "PartId" });
            DropIndex("dbo.DocumentVersions", new[] { "DocumentId" });
            DropIndex("dbo.DocumentVersions", new[] { "CreatedById" });
            DropIndex("dbo.Employees", new[] { "SystemRoleId" });
            DropIndex("dbo.Employees", new[] { "PersonId" });
            DropIndex("dbo.Employees", new[] { "UserName" });
            DropIndex("dbo.Documents", new[] { "CheckedOutById" });
            DropIndex("dbo.Parts", new[] { "Customer_Id" });
            DropIndex("dbo.Parts", new[] { "Primary3dDrawingDocumentId" });
            DropIndex("dbo.Parts", new[] { "Primary2dDrawingDocumentId" });
            DropIndex("dbo.Parts", new[] { "DrawingNumber" });
            DropIndex("dbo.Customers", new[] { "ShortName" });
            DropIndex("dbo.Customers", new[] { "FullName" });
            DropTable("dbo.PartVersions");
            DropTable("dbo.DocumentVersions");
            DropTable("dbo.SystemRoles");
            DropTable("dbo.People");
            DropTable("dbo.Employees");
            DropTable("dbo.Documents");
            DropTable("dbo.Parts");
            DropTable("dbo.Customers");
        }
    }
}
