namespace ngen.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedSomeAnnotationsAndGeneralTidying : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ProductionMethods", "Name", c => c.String(nullable: false, maxLength: 50));
            AlterStoredProcedure(
                "dbo.ProductionMethod_Insert",
                p => new
                    {
                        Name = p.String(maxLength: 50),
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
            
            AlterStoredProcedure(
                "dbo.ProductionMethod_Update",
                p => new
                    {
                        Id = p.Int(),
                        Name = p.String(maxLength: 50),
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
            
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ProductionMethods", "Name", c => c.String());
            throw new NotSupportedException("Scaffolding create or alter procedure operations is not supported in down methods.");
        }
    }
}
