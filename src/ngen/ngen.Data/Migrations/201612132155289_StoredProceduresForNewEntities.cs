namespace ngen.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StoredProceduresForNewEntities : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(
                "dbo.Supplier_Insert",
                p => new
                    {
                        FullName = p.String(maxLength: 100),
                        ShortName = p.String(maxLength: 40),
                    },
                body:
                    @"INSERT [dbo].[Suppliers]([FullName], [ShortName])
                      VALUES (@FullName, @ShortName)
                      
                      DECLARE @Id int
                      SELECT @Id = [Id]
                      FROM [dbo].[Suppliers]
                      WHERE @@ROWCOUNT > 0 AND [Id] = scope_identity()
                      
                      SELECT t0.[Id]
                      FROM [dbo].[Suppliers] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[Id] = @Id"
            );
            
            CreateStoredProcedure(
                "dbo.Supplier_Update",
                p => new
                    {
                        Id = p.Int(),
                        FullName = p.String(maxLength: 100),
                        ShortName = p.String(maxLength: 40),
                    },
                body:
                    @"UPDATE [dbo].[Suppliers]
                      SET [FullName] = @FullName, [ShortName] = @ShortName
                      WHERE ([Id] = @Id)"
            );
            
            CreateStoredProcedure(
                "dbo.Supplier_Delete",
                p => new
                    {
                        Id = p.Int(),
                    },
                body:
                    @"DELETE [dbo].[Suppliers]
                      WHERE ([Id] = @Id)"
            );
            
            CreateStoredProcedure(
                "dbo.WorkCentreGroup_Insert",
                p => new
                    {
                        Name = p.String(maxLength: 100),
                        Description = p.String(),
                        DefaultHourlyRate = p.Decimal(precision: 18, scale: 2),
                        Lineage = p.String(),
                        ParentGroupId = p.Int(),
                    },
                body:
                    @"INSERT [dbo].[WorkCentreGroups]([Name], [Description], [DefaultHourlyRate], [Lineage], [ParentGroupId])
                      VALUES (@Name, @Description, @DefaultHourlyRate, @Lineage, @ParentGroupId)
                      
                      DECLARE @Id int
                      SELECT @Id = [Id]
                      FROM [dbo].[WorkCentreGroups]
                      WHERE @@ROWCOUNT > 0 AND [Id] = scope_identity()
                      
                      SELECT t0.[Id]
                      FROM [dbo].[WorkCentreGroups] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[Id] = @Id"
            );
            
            CreateStoredProcedure(
                "dbo.WorkCentreGroup_Update",
                p => new
                    {
                        Id = p.Int(),
                        Name = p.String(maxLength: 100),
                        Description = p.String(),
                        DefaultHourlyRate = p.Decimal(precision: 18, scale: 2),
                        Lineage = p.String(),
                        ParentGroupId = p.Int(),
                    },
                body:
                    @"UPDATE [dbo].[WorkCentreGroups]
                      SET [Name] = @Name, [Description] = @Description, [DefaultHourlyRate] = @DefaultHourlyRate, [Lineage] = @Lineage, [ParentGroupId] = @ParentGroupId
                      WHERE ([Id] = @Id)"
            );
            
            CreateStoredProcedure(
                "dbo.WorkCentreGroup_Delete",
                p => new
                    {
                        Id = p.Int(),
                    },
                body:
                    @"DELETE [dbo].[WorkCentreGroups]
                      WHERE ([Id] = @Id)"
            );
            
            CreateStoredProcedure(
                "dbo.Operation_Insert",
                p => new
                    {
                        Sequence = p.Byte(),
                        Description = p.String(),
                        SetupTime = p.Double(),
                        CycleTime = p.Double(),
                        ProductionMethodId = p.Int(),
                        WorkCentreId = p.Int(),
                        WorkCentreGroupId = p.Int(),
                    },
                body:
                    @"INSERT [dbo].[Operations]([Sequence], [Description], [SetupTime], [CycleTime], [ProductionMethodId], [WorkCentreId], [WorkCentreGroupId])
                      VALUES (@Sequence, @Description, @SetupTime, @CycleTime, @ProductionMethodId, @WorkCentreId, @WorkCentreGroupId)
                      
                      DECLARE @Id int
                      SELECT @Id = [Id]
                      FROM [dbo].[Operations]
                      WHERE @@ROWCOUNT > 0 AND [Id] = scope_identity()
                      
                      SELECT t0.[Id]
                      FROM [dbo].[Operations] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[Id] = @Id"
            );
            
            CreateStoredProcedure(
                "dbo.Operation_Update",
                p => new
                    {
                        Id = p.Int(),
                        Sequence = p.Byte(),
                        Description = p.String(),
                        SetupTime = p.Double(),
                        CycleTime = p.Double(),
                        ProductionMethodId = p.Int(),
                        WorkCentreId = p.Int(),
                        WorkCentreGroupId = p.Int(),
                    },
                body:
                    @"UPDATE [dbo].[Operations]
                      SET [Sequence] = @Sequence, [Description] = @Description, [SetupTime] = @SetupTime, [CycleTime] = @CycleTime, [ProductionMethodId] = @ProductionMethodId, [WorkCentreId] = @WorkCentreId, [WorkCentreGroupId] = @WorkCentreGroupId
                      WHERE ([Id] = @Id)"
            );
            
            CreateStoredProcedure(
                "dbo.Operation_Delete",
                p => new
                    {
                        Id = p.Int(),
                    },
                body:
                    @"DELETE [dbo].[Operations]
                      WHERE ([Id] = @Id)"
            );
            
            CreateStoredProcedure(
                "dbo.ProductionMethod_Insert",
                p => new
                    {
                        Name = p.String(),
                        Description = p.String(),
                        Rating = p.Byte(),
                        CreatedAt = p.DateTime(),
                        IsProven = p.Boolean(),
                        CreatedById = p.Int(),
                        PartVersionId = p.Int(),
                    },
                body:
                    @"INSERT [dbo].[ProductionMethods]([Name], [Description], [Rating], [CreatedAt], [IsProven], [CreatedById], [PartVersionId])
                      VALUES (@Name, @Description, @Rating, @CreatedAt, @IsProven, @CreatedById, @PartVersionId)
                      
                      DECLARE @Id int
                      SELECT @Id = [Id]
                      FROM [dbo].[ProductionMethods]
                      WHERE @@ROWCOUNT > 0 AND [Id] = scope_identity()
                      
                      SELECT t0.[Id]
                      FROM [dbo].[ProductionMethods] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[Id] = @Id"
            );
            
            CreateStoredProcedure(
                "dbo.ProductionMethod_Update",
                p => new
                    {
                        Id = p.Int(),
                        Name = p.String(),
                        Description = p.String(),
                        Rating = p.Byte(),
                        CreatedAt = p.DateTime(),
                        IsProven = p.Boolean(),
                        CreatedById = p.Int(),
                        PartVersionId = p.Int(),
                    },
                body:
                    @"UPDATE [dbo].[ProductionMethods]
                      SET [Name] = @Name, [Description] = @Description, [Rating] = @Rating, [CreatedAt] = @CreatedAt, [IsProven] = @IsProven, [CreatedById] = @CreatedById, [PartVersionId] = @PartVersionId
                      WHERE ([Id] = @Id)"
            );
            
            CreateStoredProcedure(
                "dbo.ProductionMethod_Delete",
                p => new
                    {
                        Id = p.Int(),
                    },
                body:
                    @"DELETE [dbo].[ProductionMethods]
                      WHERE ([Id] = @Id)"
            );
            
            CreateStoredProcedure(
                "dbo.WorkCentre_Insert",
                p => new
                    {
                        Name = p.String(maxLength: 100),
                        Caption = p.String(maxLength: 50),
                        WorkCentreGroupId = p.Int(),
                    },
                body:
                    @"INSERT [dbo].[WorkCentres]([Name], [Caption], [WorkCentreGroupId])
                      VALUES (@Name, @Caption, @WorkCentreGroupId)
                      
                      DECLARE @Id int
                      SELECT @Id = [Id]
                      FROM [dbo].[WorkCentres]
                      WHERE @@ROWCOUNT > 0 AND [Id] = scope_identity()
                      
                      SELECT t0.[Id]
                      FROM [dbo].[WorkCentres] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[Id] = @Id"
            );
            
            CreateStoredProcedure(
                "dbo.WorkCentre_Update",
                p => new
                    {
                        Id = p.Int(),
                        Name = p.String(maxLength: 100),
                        Caption = p.String(maxLength: 50),
                        WorkCentreGroupId = p.Int(),
                    },
                body:
                    @"UPDATE [dbo].[WorkCentres]
                      SET [Name] = @Name, [Caption] = @Caption, [WorkCentreGroupId] = @WorkCentreGroupId
                      WHERE ([Id] = @Id)"
            );
            
            CreateStoredProcedure(
                "dbo.WorkCentre_Delete",
                p => new
                    {
                        Id = p.Int(),
                    },
                body:
                    @"DELETE [dbo].[WorkCentres]
                      WHERE ([Id] = @Id)"
            );
            
        }
        
        public override void Down()
        {
            DropStoredProcedure("dbo.WorkCentre_Delete");
            DropStoredProcedure("dbo.WorkCentre_Update");
            DropStoredProcedure("dbo.WorkCentre_Insert");
            DropStoredProcedure("dbo.ProductionMethod_Delete");
            DropStoredProcedure("dbo.ProductionMethod_Update");
            DropStoredProcedure("dbo.ProductionMethod_Insert");
            DropStoredProcedure("dbo.Operation_Delete");
            DropStoredProcedure("dbo.Operation_Update");
            DropStoredProcedure("dbo.Operation_Insert");
            DropStoredProcedure("dbo.WorkCentreGroup_Delete");
            DropStoredProcedure("dbo.WorkCentreGroup_Update");
            DropStoredProcedure("dbo.WorkCentreGroup_Insert");
            DropStoredProcedure("dbo.Supplier_Delete");
            DropStoredProcedure("dbo.Supplier_Update");
            DropStoredProcedure("dbo.Supplier_Insert");
        }
    }
}
