namespace ngen.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ICollectionDocumentRemovedFromPart : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Documents", "Part_Id", "dbo.Parts");
            DropIndex("dbo.Documents", new[] { "Part_Id" });
            DropColumn("dbo.Documents", "Part_Id");
            AlterStoredProcedure(
                "dbo.Document_Insert",
                p => new
                    {
                        FileName = p.String(maxLength: 100),
                        Permissions = p.Binary(),
                        DocumentFolderId = p.Int(),
                        PartId = p.Int(),
                        PartVersionId = p.Int(),
                        CheckedOutById = p.Int(),
                        CheckedOutAt = p.DateTime(),
                    },
                body:
                    @"INSERT [dbo].[Documents]([FileName], [Permissions], [DocumentFolderId], [PartId], [PartVersionId], [CheckedOutById], [CheckedOutAt])
                      VALUES (@FileName, @Permissions, @DocumentFolderId, @PartId, @PartVersionId, @CheckedOutById, @CheckedOutAt)
                      
                      DECLARE @Id int
                      SELECT @Id = [Id]
                      FROM [dbo].[Documents]
                      WHERE @@ROWCOUNT > 0 AND [Id] = scope_identity()
                      
                      SELECT t0.[Id]
                      FROM [dbo].[Documents] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[Id] = @Id"
            );
            
            AlterStoredProcedure(
                "dbo.Document_Update",
                p => new
                    {
                        Id = p.Int(),
                        FileName = p.String(maxLength: 100),
                        Permissions = p.Binary(),
                        DocumentFolderId = p.Int(),
                        PartId = p.Int(),
                        PartVersionId = p.Int(),
                        CheckedOutById = p.Int(),
                        CheckedOutAt = p.DateTime(),
                    },
                body:
                    @"UPDATE [dbo].[Documents]
                      SET [FileName] = @FileName, [Permissions] = @Permissions, [DocumentFolderId] = @DocumentFolderId, [PartId] = @PartId, [PartVersionId] = @PartVersionId, [CheckedOutById] = @CheckedOutById, [CheckedOutAt] = @CheckedOutAt
                      WHERE ([Id] = @Id)"
            );
            
            AlterStoredProcedure(
                "dbo.Document_Delete",
                p => new
                    {
                        Id = p.Int(),
                    },
                body:
                    @"DELETE [dbo].[Documents]
                      WHERE ([Id] = @Id)"
            );
            
        }
        
        public override void Down()
        {
            AddColumn("dbo.Documents", "Part_Id", c => c.Int());
            CreateIndex("dbo.Documents", "Part_Id");
            AddForeignKey("dbo.Documents", "Part_Id", "dbo.Parts", "Id");
            throw new NotSupportedException("Scaffolding create or alter procedure operations is not supported in down methods.");
        }
    }
}
