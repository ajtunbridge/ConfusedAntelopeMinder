namespace ngen.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AssignedFixturesToOperationsInsteadOfParts : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Fixtures", "PartId", "dbo.Parts");
            DropIndex("dbo.Fixtures", new[] { "PartId" });
            AddColumn("dbo.Fixtures", "OperationId", c => c.Int());
            CreateIndex("dbo.Fixtures", "OperationId");
            AddForeignKey("dbo.Fixtures", "OperationId", "dbo.Operations", "Id");
            DropColumn("dbo.Fixtures", "PartId");
            AlterStoredProcedure(
                "dbo.Fixture_Insert",
                p => new
                    {
                        Caption = p.String(maxLength: 100),
                        Description = p.String(),
                        Location = p.String(maxLength: 100),
                        OperationId = p.Int(),
                    },
                body:
                    @"INSERT [dbo].[Fixtures]([Caption], [Description], [Location], [OperationId])
                      VALUES (@Caption, @Description, @Location, @OperationId)
                      
                      DECLARE @Id int
                      SELECT @Id = [Id]
                      FROM [dbo].[Fixtures]
                      WHERE @@ROWCOUNT > 0 AND [Id] = scope_identity()
                      
                      SELECT t0.[Id]
                      FROM [dbo].[Fixtures] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[Id] = @Id"
            );
            
            AlterStoredProcedure(
                "dbo.Fixture_Update",
                p => new
                    {
                        Id = p.Int(),
                        Caption = p.String(maxLength: 100),
                        Description = p.String(),
                        Location = p.String(maxLength: 100),
                        OperationId = p.Int(),
                    },
                body:
                    @"UPDATE [dbo].[Fixtures]
                      SET [Caption] = @Caption, [Description] = @Description, [Location] = @Location, [OperationId] = @OperationId
                      WHERE ([Id] = @Id)"
            );
            
        }
        
        public override void Down()
        {
            AddColumn("dbo.Fixtures", "PartId", c => c.Int(nullable: false));
            DropForeignKey("dbo.Fixtures", "OperationId", "dbo.Operations");
            DropIndex("dbo.Fixtures", new[] { "OperationId" });
            DropColumn("dbo.Fixtures", "OperationId");
            CreateIndex("dbo.Fixtures", "PartId");
            AddForeignKey("dbo.Fixtures", "PartId", "dbo.Parts", "Id", cascadeDelete: true);
            throw new NotSupportedException("Scaffolding create or alter procedure operations is not supported in down methods.");
        }
    }
}
