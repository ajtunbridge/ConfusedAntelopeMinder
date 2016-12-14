namespace ngen.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BCryptPasswordAndNewEntities : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Parts", "Customer_Id", "dbo.Customers");
            DropIndex("dbo.Parts", new[] { "Customer_Id" });
            RenameColumn(table: "dbo.Parts", name: "Customer_Id", newName: "CustomerId");
            CreateTable(
                "dbo.ClientSettings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CompanyName = c.String(nullable: false, maxLength: 100),
                        DocumentStorageTechnology = c.String(nullable: false, maxLength: 5),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.CompanyName, unique: true);
            
            CreateTable(
                "dbo.DocumentFolders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        Description = c.String(),
                        ParentId = c.Int(),
                        ClientSettingId = c.Int(),
                        CustomerId = c.Int(),
                        EmployeeId = c.Int(),
                        PartId = c.Int(),
                        PartVersionId = c.Int(),
                        SupplierId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ClientSettings", t => t.ClientSettingId)
                .ForeignKey("dbo.Customers", t => t.CustomerId)
                .ForeignKey("dbo.Employees", t => t.EmployeeId)
                .ForeignKey("dbo.DocumentFolders", t => t.ParentId)
                .ForeignKey("dbo.Parts", t => t.PartId)
                .ForeignKey("dbo.PartVersions", t => t.PartVersionId)
                .ForeignKey("dbo.Suppliers", t => t.SupplierId)
                .Index(t => t.ParentId)
                .Index(t => t.ClientSettingId)
                .Index(t => t.CustomerId)
                .Index(t => t.EmployeeId)
                .Index(t => t.PartId)
                .Index(t => t.PartVersionId)
                .Index(t => t.SupplierId);
            
            CreateTable(
                "dbo.Suppliers",
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
                "dbo.Fixtures",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Caption = c.String(nullable: false, maxLength: 100),
                        Description = c.String(),
                        Location = c.String(nullable: false, maxLength: 100),
                        PartId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Parts", t => t.PartId, cascadeDelete: true)
                .Index(t => t.PartId);
            
            CreateTable(
                "dbo.Photos",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Address = c.String(nullable: false, maxLength: 100),
                        Caption = c.String(maxLength: 50),
                        Description = c.String(),
                        FileName = c.String(maxLength: 100),
                        IsPrimary = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Documents", "DocumentFolderId", c => c.Int());
            AddColumn("dbo.Documents", "PartId", c => c.Int());
            AddColumn("dbo.Documents", "PartVersionId", c => c.Int());
            AddColumn("dbo.Employees", "Password", c => c.String(nullable: false, maxLength: 60, fixedLength: true));
            AlterColumn("dbo.Parts", "CustomerId", c => c.Int(nullable: false));
            CreateIndex("dbo.Parts", "CustomerId");
            CreateIndex("dbo.Documents", "DocumentFolderId");
            CreateIndex("dbo.Documents", "PartId");
            CreateIndex("dbo.Documents", "PartVersionId");
            AddForeignKey("dbo.Documents", "DocumentFolderId", "dbo.DocumentFolders", "Id");
            AddForeignKey("dbo.Documents", "PartId", "dbo.Parts", "Id");
            AddForeignKey("dbo.Documents", "PartVersionId", "dbo.PartVersions", "Id");
            AddForeignKey("dbo.Parts", "CustomerId", "dbo.Customers", "Id", cascadeDelete: true);
            DropColumn("dbo.Employees", "PasswordHash");
            DropColumn("dbo.Employees", "PasswordSalt");
            CreateStoredProcedure(
                "dbo.ClientSetting_Insert",
                p => new
                    {
                        CompanyName = p.String(maxLength: 100),
                        DocumentStorageTechnology = p.String(maxLength: 5),
                    },
                body:
                    @"INSERT [dbo].[ClientSettings]([CompanyName], [DocumentStorageTechnology])
                      VALUES (@CompanyName, @DocumentStorageTechnology)
                      
                      DECLARE @Id int
                      SELECT @Id = [Id]
                      FROM [dbo].[ClientSettings]
                      WHERE @@ROWCOUNT > 0 AND [Id] = scope_identity()
                      
                      SELECT t0.[Id]
                      FROM [dbo].[ClientSettings] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[Id] = @Id"
            );
            
            CreateStoredProcedure(
                "dbo.ClientSetting_Update",
                p => new
                    {
                        Id = p.Int(),
                        CompanyName = p.String(maxLength: 100),
                        DocumentStorageTechnology = p.String(maxLength: 5),
                    },
                body:
                    @"UPDATE [dbo].[ClientSettings]
                      SET [CompanyName] = @CompanyName, [DocumentStorageTechnology] = @DocumentStorageTechnology
                      WHERE ([Id] = @Id)"
            );
            
            CreateStoredProcedure(
                "dbo.ClientSetting_Delete",
                p => new
                    {
                        Id = p.Int(),
                    },
                body:
                    @"DELETE [dbo].[ClientSettings]
                      WHERE ([Id] = @Id)"
            );
            
            CreateStoredProcedure(
                "dbo.Customer_Insert",
                p => new
                    {
                        FullName = p.String(maxLength: 100),
                        ShortName = p.String(maxLength: 40),
                    },
                body:
                    @"INSERT [dbo].[Customers]([FullName], [ShortName])
                      VALUES (@FullName, @ShortName)
                      
                      DECLARE @Id int
                      SELECT @Id = [Id]
                      FROM [dbo].[Customers]
                      WHERE @@ROWCOUNT > 0 AND [Id] = scope_identity()
                      
                      SELECT t0.[Id]
                      FROM [dbo].[Customers] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[Id] = @Id"
            );
            
            CreateStoredProcedure(
                "dbo.Customer_Update",
                p => new
                    {
                        Id = p.Int(),
                        FullName = p.String(maxLength: 100),
                        ShortName = p.String(maxLength: 40),
                    },
                body:
                    @"UPDATE [dbo].[Customers]
                      SET [FullName] = @FullName, [ShortName] = @ShortName
                      WHERE ([Id] = @Id)"
            );
            
            CreateStoredProcedure(
                "dbo.Customer_Delete",
                p => new
                    {
                        Id = p.Int(),
                    },
                body:
                    @"DELETE [dbo].[Customers]
                      WHERE ([Id] = @Id)"
            );
            
            CreateStoredProcedure(
                "dbo.Part_Insert",
                p => new
                    {
                        DrawingNumber = p.String(maxLength: 50),
                        Name = p.String(maxLength: 100),
                        CustomerId = p.Int(),
                        Primary2dDrawingDocumentId = p.Int(),
                        Primary3dDrawingDocumentId = p.Int(),
                    },
                body:
                    @"INSERT [dbo].[Parts]([DrawingNumber], [Name], [CustomerId], [Primary2dDrawingDocumentId], [Primary3dDrawingDocumentId])
                      VALUES (@DrawingNumber, @Name, @CustomerId, @Primary2dDrawingDocumentId, @Primary3dDrawingDocumentId)
                      
                      DECLARE @Id int
                      SELECT @Id = [Id]
                      FROM [dbo].[Parts]
                      WHERE @@ROWCOUNT > 0 AND [Id] = scope_identity()
                      
                      SELECT t0.[Id]
                      FROM [dbo].[Parts] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[Id] = @Id"
            );
            
            CreateStoredProcedure(
                "dbo.Part_Update",
                p => new
                    {
                        Id = p.Int(),
                        DrawingNumber = p.String(maxLength: 50),
                        Name = p.String(maxLength: 100),
                        CustomerId = p.Int(),
                        Primary2dDrawingDocumentId = p.Int(),
                        Primary3dDrawingDocumentId = p.Int(),
                    },
                body:
                    @"UPDATE [dbo].[Parts]
                      SET [DrawingNumber] = @DrawingNumber, [Name] = @Name, [CustomerId] = @CustomerId, [Primary2dDrawingDocumentId] = @Primary2dDrawingDocumentId, [Primary3dDrawingDocumentId] = @Primary3dDrawingDocumentId
                      WHERE ([Id] = @Id)"
            );
            
            CreateStoredProcedure(
                "dbo.Part_Delete",
                p => new
                    {
                        Id = p.Int(),
                    },
                body:
                    @"DELETE [dbo].[Parts]
                      WHERE ([Id] = @Id)"
            );
            
            CreateStoredProcedure(
                "dbo.Document_Insert",
                p => new
                    {
                        FileName = p.String(maxLength: 100),
                        IsCheckedOut = p.Boolean(),
                        Permissions = p.Binary(),
                        DocumentFolderId = p.Int(),
                        PartId = p.Int(),
                        PartVersionId = p.Int(),
                        CheckedOutById = p.Int(),
                        CheckedOutAt = p.DateTime(),
                    },
                body:
                    @"INSERT [dbo].[Documents]([FileName], [IsCheckedOut], [Permissions], [DocumentFolderId], [PartId], [PartVersionId], [CheckedOutById], [CheckedOutAt])
                      VALUES (@FileName, @IsCheckedOut, @Permissions, @DocumentFolderId, @PartId, @PartVersionId, @CheckedOutById, @CheckedOutAt)
                      
                      DECLARE @Id int
                      SELECT @Id = [Id]
                      FROM [dbo].[Documents]
                      WHERE @@ROWCOUNT > 0 AND [Id] = scope_identity()
                      
                      SELECT t0.[Id]
                      FROM [dbo].[Documents] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[Id] = @Id"
            );
            
            CreateStoredProcedure(
                "dbo.Document_Update",
                p => new
                    {
                        Id = p.Int(),
                        FileName = p.String(maxLength: 100),
                        IsCheckedOut = p.Boolean(),
                        Permissions = p.Binary(),
                        DocumentFolderId = p.Int(),
                        PartId = p.Int(),
                        PartVersionId = p.Int(),
                        CheckedOutById = p.Int(),
                        CheckedOutAt = p.DateTime(),
                    },
                body:
                    @"UPDATE [dbo].[Documents]
                      SET [FileName] = @FileName, [IsCheckedOut] = @IsCheckedOut, [Permissions] = @Permissions, [DocumentFolderId] = @DocumentFolderId, [PartId] = @PartId, [PartVersionId] = @PartVersionId, [CheckedOutById] = @CheckedOutById, [CheckedOutAt] = @CheckedOutAt
                      WHERE ([Id] = @Id)"
            );
            
            CreateStoredProcedure(
                "dbo.Document_Delete",
                p => new
                    {
                        Id = p.Int(),
                    },
                body:
                    @"DELETE [dbo].[Documents]
                      WHERE ([Id] = @Id)"
            );
            
            CreateStoredProcedure(
                "dbo.Employee_Insert",
                p => new
                    {
                        UserName = p.String(maxLength: 30),
                        Password = p.String(maxLength: 60, fixedLength: true),
                        IsActive = p.Boolean(),
                        PersonId = p.Int(),
                        SystemRoleId = p.Int(),
                    },
                body:
                    @"INSERT [dbo].[Employees]([UserName], [Password], [IsActive], [PersonId], [SystemRoleId])
                      VALUES (@UserName, @Password, @IsActive, @PersonId, @SystemRoleId)
                      
                      DECLARE @Id int
                      SELECT @Id = [Id]
                      FROM [dbo].[Employees]
                      WHERE @@ROWCOUNT > 0 AND [Id] = scope_identity()
                      
                      SELECT t0.[Id]
                      FROM [dbo].[Employees] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[Id] = @Id"
            );
            
            CreateStoredProcedure(
                "dbo.Employee_Update",
                p => new
                    {
                        Id = p.Int(),
                        UserName = p.String(maxLength: 30),
                        Password = p.String(maxLength: 60, fixedLength: true),
                        IsActive = p.Boolean(),
                        PersonId = p.Int(),
                        SystemRoleId = p.Int(),
                    },
                body:
                    @"UPDATE [dbo].[Employees]
                      SET [UserName] = @UserName, [Password] = @Password, [IsActive] = @IsActive, [PersonId] = @PersonId, [SystemRoleId] = @SystemRoleId
                      WHERE ([Id] = @Id)"
            );
            
            CreateStoredProcedure(
                "dbo.Employee_Delete",
                p => new
                    {
                        Id = p.Int(),
                    },
                body:
                    @"DELETE [dbo].[Employees]
                      WHERE ([Id] = @Id)"
            );
            
            CreateStoredProcedure(
                "dbo.Person_Insert",
                p => new
                    {
                        Title = p.String(),
                        FirstName = p.String(maxLength: 30),
                        MiddleName = p.String(),
                        LastName = p.String(maxLength: 30),
                        DateOfBirth = p.DateTime(),
                    },
                body:
                    @"INSERT [dbo].[People]([Title], [FirstName], [MiddleName], [LastName], [DateOfBirth])
                      VALUES (@Title, @FirstName, @MiddleName, @LastName, @DateOfBirth)
                      
                      DECLARE @Id int
                      SELECT @Id = [Id]
                      FROM [dbo].[People]
                      WHERE @@ROWCOUNT > 0 AND [Id] = scope_identity()
                      
                      SELECT t0.[Id]
                      FROM [dbo].[People] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[Id] = @Id"
            );
            
            CreateStoredProcedure(
                "dbo.Person_Update",
                p => new
                    {
                        Id = p.Int(),
                        Title = p.String(),
                        FirstName = p.String(maxLength: 30),
                        MiddleName = p.String(),
                        LastName = p.String(maxLength: 30),
                        DateOfBirth = p.DateTime(),
                    },
                body:
                    @"UPDATE [dbo].[People]
                      SET [Title] = @Title, [FirstName] = @FirstName, [MiddleName] = @MiddleName, [LastName] = @LastName, [DateOfBirth] = @DateOfBirth
                      WHERE ([Id] = @Id)"
            );
            
            CreateStoredProcedure(
                "dbo.Person_Delete",
                p => new
                    {
                        Id = p.Int(),
                    },
                body:
                    @"DELETE [dbo].[People]
                      WHERE ([Id] = @Id)"
            );
            
            CreateStoredProcedure(
                "dbo.SystemRole_Insert",
                p => new
                    {
                        Name = p.String(maxLength: 30),
                        Description = p.String(),
                        Permissions = p.Binary(),
                    },
                body:
                    @"INSERT [dbo].[SystemRoles]([Name], [Description], [Permissions])
                      VALUES (@Name, @Description, @Permissions)
                      
                      DECLARE @Id int
                      SELECT @Id = [Id]
                      FROM [dbo].[SystemRoles]
                      WHERE @@ROWCOUNT > 0 AND [Id] = scope_identity()
                      
                      SELECT t0.[Id]
                      FROM [dbo].[SystemRoles] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[Id] = @Id"
            );
            
            CreateStoredProcedure(
                "dbo.SystemRole_Update",
                p => new
                    {
                        Id = p.Int(),
                        Name = p.String(maxLength: 30),
                        Description = p.String(),
                        Permissions = p.Binary(),
                    },
                body:
                    @"UPDATE [dbo].[SystemRoles]
                      SET [Name] = @Name, [Description] = @Description, [Permissions] = @Permissions
                      WHERE ([Id] = @Id)"
            );
            
            CreateStoredProcedure(
                "dbo.SystemRole_Delete",
                p => new
                    {
                        Id = p.Int(),
                    },
                body:
                    @"DELETE [dbo].[SystemRoles]
                      WHERE ([Id] = @Id)"
            );
            
            CreateStoredProcedure(
                "dbo.DocumentFolder_Insert",
                p => new
                    {
                        Name = p.String(maxLength: 50),
                        Description = p.String(),
                        ParentId = p.Int(),
                        ClientSettingId = p.Int(),
                        CustomerId = p.Int(),
                        EmployeeId = p.Int(),
                        PartId = p.Int(),
                        PartVersionId = p.Int(),
                        SupplierId = p.Int(),
                    },
                body:
                    @"INSERT [dbo].[DocumentFolders]([Name], [Description], [ParentId], [ClientSettingId], [CustomerId], [EmployeeId], [PartId], [PartVersionId], [SupplierId])
                      VALUES (@Name, @Description, @ParentId, @ClientSettingId, @CustomerId, @EmployeeId, @PartId, @PartVersionId, @SupplierId)
                      
                      DECLARE @Id int
                      SELECT @Id = [Id]
                      FROM [dbo].[DocumentFolders]
                      WHERE @@ROWCOUNT > 0 AND [Id] = scope_identity()
                      
                      SELECT t0.[Id]
                      FROM [dbo].[DocumentFolders] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[Id] = @Id"
            );
            
            CreateStoredProcedure(
                "dbo.DocumentFolder_Update",
                p => new
                    {
                        Id = p.Int(),
                        Name = p.String(maxLength: 50),
                        Description = p.String(),
                        ParentId = p.Int(),
                        ClientSettingId = p.Int(),
                        CustomerId = p.Int(),
                        EmployeeId = p.Int(),
                        PartId = p.Int(),
                        PartVersionId = p.Int(),
                        SupplierId = p.Int(),
                    },
                body:
                    @"UPDATE [dbo].[DocumentFolders]
                      SET [Name] = @Name, [Description] = @Description, [ParentId] = @ParentId, [ClientSettingId] = @ClientSettingId, [CustomerId] = @CustomerId, [EmployeeId] = @EmployeeId, [PartId] = @PartId, [PartVersionId] = @PartVersionId, [SupplierId] = @SupplierId
                      WHERE ([Id] = @Id)"
            );
            
            CreateStoredProcedure(
                "dbo.DocumentFolder_Delete",
                p => new
                    {
                        Id = p.Int(),
                    },
                body:
                    @"DELETE [dbo].[DocumentFolders]
                      WHERE ([Id] = @Id)"
            );
            
            CreateStoredProcedure(
                "dbo.PartVersion_Insert",
                p => new
                    {
                        VersionNumber = p.String(maxLength: 12),
                        Changes = p.String(),
                        PartId = p.Int(),
                    },
                body:
                    @"INSERT [dbo].[PartVersions]([VersionNumber], [Changes], [PartId])
                      VALUES (@VersionNumber, @Changes, @PartId)
                      
                      DECLARE @Id int
                      SELECT @Id = [Id]
                      FROM [dbo].[PartVersions]
                      WHERE @@ROWCOUNT > 0 AND [Id] = scope_identity()
                      
                      SELECT t0.[Id]
                      FROM [dbo].[PartVersions] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[Id] = @Id"
            );
            
            CreateStoredProcedure(
                "dbo.PartVersion_Update",
                p => new
                    {
                        Id = p.Int(),
                        VersionNumber = p.String(maxLength: 12),
                        Changes = p.String(),
                        PartId = p.Int(),
                    },
                body:
                    @"UPDATE [dbo].[PartVersions]
                      SET [VersionNumber] = @VersionNumber, [Changes] = @Changes, [PartId] = @PartId
                      WHERE ([Id] = @Id)"
            );
            
            CreateStoredProcedure(
                "dbo.PartVersion_Delete",
                p => new
                    {
                        Id = p.Int(),
                    },
                body:
                    @"DELETE [dbo].[PartVersions]
                      WHERE ([Id] = @Id)"
            );
            
            CreateStoredProcedure(
                "dbo.DocumentVersion_Insert",
                p => new
                    {
                        Hash = p.String(maxLength: 32, fixedLength: true),
                        Changes = p.String(),
                        CreatedAt = p.DateTime(),
                        CreatedById = p.Int(),
                        DocumentId = p.Int(),
                    },
                body:
                    @"INSERT [dbo].[DocumentVersions]([Hash], [Changes], [CreatedAt], [CreatedById], [DocumentId])
                      VALUES (@Hash, @Changes, @CreatedAt, @CreatedById, @DocumentId)
                      
                      DECLARE @Id int
                      SELECT @Id = [Id]
                      FROM [dbo].[DocumentVersions]
                      WHERE @@ROWCOUNT > 0 AND [Id] = scope_identity()
                      
                      SELECT t0.[Id]
                      FROM [dbo].[DocumentVersions] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[Id] = @Id"
            );
            
            CreateStoredProcedure(
                "dbo.DocumentVersion_Update",
                p => new
                    {
                        Id = p.Int(),
                        Hash = p.String(maxLength: 32, fixedLength: true),
                        Changes = p.String(),
                        CreatedAt = p.DateTime(),
                        CreatedById = p.Int(),
                        DocumentId = p.Int(),
                    },
                body:
                    @"UPDATE [dbo].[DocumentVersions]
                      SET [Hash] = @Hash, [Changes] = @Changes, [CreatedAt] = @CreatedAt, [CreatedById] = @CreatedById, [DocumentId] = @DocumentId
                      WHERE ([Id] = @Id)"
            );
            
            CreateStoredProcedure(
                "dbo.DocumentVersion_Delete",
                p => new
                    {
                        Id = p.Int(),
                    },
                body:
                    @"DELETE [dbo].[DocumentVersions]
                      WHERE ([Id] = @Id)"
            );
            
            CreateStoredProcedure(
                "dbo.Fixture_Insert",
                p => new
                    {
                        Caption = p.String(maxLength: 100),
                        Description = p.String(),
                        Location = p.String(maxLength: 100),
                        PartId = p.Int(),
                    },
                body:
                    @"INSERT [dbo].[Fixtures]([Caption], [Description], [Location], [PartId])
                      VALUES (@Caption, @Description, @Location, @PartId)
                      
                      DECLARE @Id int
                      SELECT @Id = [Id]
                      FROM [dbo].[Fixtures]
                      WHERE @@ROWCOUNT > 0 AND [Id] = scope_identity()
                      
                      SELECT t0.[Id]
                      FROM [dbo].[Fixtures] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[Id] = @Id"
            );
            
            CreateStoredProcedure(
                "dbo.Fixture_Update",
                p => new
                    {
                        Id = p.Int(),
                        Caption = p.String(maxLength: 100),
                        Description = p.String(),
                        Location = p.String(maxLength: 100),
                        PartId = p.Int(),
                    },
                body:
                    @"UPDATE [dbo].[Fixtures]
                      SET [Caption] = @Caption, [Description] = @Description, [Location] = @Location, [PartId] = @PartId
                      WHERE ([Id] = @Id)"
            );
            
            CreateStoredProcedure(
                "dbo.Fixture_Delete",
                p => new
                    {
                        Id = p.Int(),
                    },
                body:
                    @"DELETE [dbo].[Fixtures]
                      WHERE ([Id] = @Id)"
            );
            
            CreateStoredProcedure(
                "dbo.Photo_Insert",
                p => new
                    {
                        Address = p.String(maxLength: 100),
                        Caption = p.String(maxLength: 50),
                        Description = p.String(),
                        FileName = p.String(maxLength: 100),
                        IsPrimary = p.Boolean(),
                    },
                body:
                    @"INSERT [dbo].[Photos]([Address], [Caption], [Description], [FileName], [IsPrimary])
                      VALUES (@Address, @Caption, @Description, @FileName, @IsPrimary)
                      
                      DECLARE @Id int
                      SELECT @Id = [Id]
                      FROM [dbo].[Photos]
                      WHERE @@ROWCOUNT > 0 AND [Id] = scope_identity()
                      
                      SELECT t0.[Id]
                      FROM [dbo].[Photos] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[Id] = @Id"
            );
            
            CreateStoredProcedure(
                "dbo.Photo_Update",
                p => new
                    {
                        Id = p.Int(),
                        Address = p.String(maxLength: 100),
                        Caption = p.String(maxLength: 50),
                        Description = p.String(),
                        FileName = p.String(maxLength: 100),
                        IsPrimary = p.Boolean(),
                    },
                body:
                    @"UPDATE [dbo].[Photos]
                      SET [Address] = @Address, [Caption] = @Caption, [Description] = @Description, [FileName] = @FileName, [IsPrimary] = @IsPrimary
                      WHERE ([Id] = @Id)"
            );
            
            CreateStoredProcedure(
                "dbo.Photo_Delete",
                p => new
                    {
                        Id = p.Int(),
                    },
                body:
                    @"DELETE [dbo].[Photos]
                      WHERE ([Id] = @Id)"
            );
            
        }
        
        public override void Down()
        {
            DropStoredProcedure("dbo.Photo_Delete");
            DropStoredProcedure("dbo.Photo_Update");
            DropStoredProcedure("dbo.Photo_Insert");
            DropStoredProcedure("dbo.Fixture_Delete");
            DropStoredProcedure("dbo.Fixture_Update");
            DropStoredProcedure("dbo.Fixture_Insert");
            DropStoredProcedure("dbo.DocumentVersion_Delete");
            DropStoredProcedure("dbo.DocumentVersion_Update");
            DropStoredProcedure("dbo.DocumentVersion_Insert");
            DropStoredProcedure("dbo.PartVersion_Delete");
            DropStoredProcedure("dbo.PartVersion_Update");
            DropStoredProcedure("dbo.PartVersion_Insert");
            DropStoredProcedure("dbo.DocumentFolder_Delete");
            DropStoredProcedure("dbo.DocumentFolder_Update");
            DropStoredProcedure("dbo.DocumentFolder_Insert");
            DropStoredProcedure("dbo.SystemRole_Delete");
            DropStoredProcedure("dbo.SystemRole_Update");
            DropStoredProcedure("dbo.SystemRole_Insert");
            DropStoredProcedure("dbo.Person_Delete");
            DropStoredProcedure("dbo.Person_Update");
            DropStoredProcedure("dbo.Person_Insert");
            DropStoredProcedure("dbo.Employee_Delete");
            DropStoredProcedure("dbo.Employee_Update");
            DropStoredProcedure("dbo.Employee_Insert");
            DropStoredProcedure("dbo.Document_Delete");
            DropStoredProcedure("dbo.Document_Update");
            DropStoredProcedure("dbo.Document_Insert");
            DropStoredProcedure("dbo.Part_Delete");
            DropStoredProcedure("dbo.Part_Update");
            DropStoredProcedure("dbo.Part_Insert");
            DropStoredProcedure("dbo.Customer_Delete");
            DropStoredProcedure("dbo.Customer_Update");
            DropStoredProcedure("dbo.Customer_Insert");
            DropStoredProcedure("dbo.ClientSetting_Delete");
            DropStoredProcedure("dbo.ClientSetting_Update");
            DropStoredProcedure("dbo.ClientSetting_Insert");
            AddColumn("dbo.Employees", "PasswordSalt", c => c.String(nullable: false, maxLength: 24, fixedLength: true));
            AddColumn("dbo.Employees", "PasswordHash", c => c.String(nullable: false, maxLength: 88, fixedLength: true));
            DropForeignKey("dbo.Parts", "CustomerId", "dbo.Customers");
            DropForeignKey("dbo.Fixtures", "PartId", "dbo.Parts");
            DropForeignKey("dbo.Documents", "PartVersionId", "dbo.PartVersions");
            DropForeignKey("dbo.Documents", "PartId", "dbo.Parts");
            DropForeignKey("dbo.Documents", "DocumentFolderId", "dbo.DocumentFolders");
            DropForeignKey("dbo.DocumentFolders", "SupplierId", "dbo.Suppliers");
            DropForeignKey("dbo.DocumentFolders", "PartVersionId", "dbo.PartVersions");
            DropForeignKey("dbo.DocumentFolders", "PartId", "dbo.Parts");
            DropForeignKey("dbo.DocumentFolders", "ParentId", "dbo.DocumentFolders");
            DropForeignKey("dbo.DocumentFolders", "EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.DocumentFolders", "CustomerId", "dbo.Customers");
            DropForeignKey("dbo.DocumentFolders", "ClientSettingId", "dbo.ClientSettings");
            DropIndex("dbo.Fixtures", new[] { "PartId" });
            DropIndex("dbo.Suppliers", new[] { "ShortName" });
            DropIndex("dbo.Suppliers", new[] { "FullName" });
            DropIndex("dbo.DocumentFolders", new[] { "SupplierId" });
            DropIndex("dbo.DocumentFolders", new[] { "PartVersionId" });
            DropIndex("dbo.DocumentFolders", new[] { "PartId" });
            DropIndex("dbo.DocumentFolders", new[] { "EmployeeId" });
            DropIndex("dbo.DocumentFolders", new[] { "CustomerId" });
            DropIndex("dbo.DocumentFolders", new[] { "ClientSettingId" });
            DropIndex("dbo.DocumentFolders", new[] { "ParentId" });
            DropIndex("dbo.Documents", new[] { "PartVersionId" });
            DropIndex("dbo.Documents", new[] { "PartId" });
            DropIndex("dbo.Documents", new[] { "DocumentFolderId" });
            DropIndex("dbo.Parts", new[] { "CustomerId" });
            DropIndex("dbo.ClientSettings", new[] { "CompanyName" });
            AlterColumn("dbo.Parts", "CustomerId", c => c.Int());
            DropColumn("dbo.Employees", "Password");
            DropColumn("dbo.Documents", "PartVersionId");
            DropColumn("dbo.Documents", "PartId");
            DropColumn("dbo.Documents", "DocumentFolderId");
            DropTable("dbo.Photos");
            DropTable("dbo.Fixtures");
            DropTable("dbo.Suppliers");
            DropTable("dbo.DocumentFolders");
            DropTable("dbo.ClientSettings");
            RenameColumn(table: "dbo.Parts", name: "CustomerId", newName: "Customer_Id");
            CreateIndex("dbo.Parts", "Customer_Id");
            AddForeignKey("dbo.Parts", "Customer_Id", "dbo.Customers", "Id");
        }
    }
}
