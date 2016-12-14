namespace ngen.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NotSureWhatIChangedTBH : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Documents", "IsCheckedOut");
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
                        Part_Id = p.Int(),
                    },
                body:
                    @"INSERT [dbo].[Documents]([FileName], [Permissions], [DocumentFolderId], [PartId], [PartVersionId], [CheckedOutById], [CheckedOutAt], [Part_Id])
                      VALUES (@FileName, @Permissions, @DocumentFolderId, @PartId, @PartVersionId, @CheckedOutById, @CheckedOutAt, @Part_Id)
                      
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
                        Part_Id = p.Int(),
                    },
                body:
                    @"UPDATE [dbo].[Documents]
                      SET [FileName] = @FileName, [Permissions] = @Permissions, [DocumentFolderId] = @DocumentFolderId, [PartId] = @PartId, [PartVersionId] = @PartVersionId, [CheckedOutById] = @CheckedOutById, [CheckedOutAt] = @CheckedOutAt, [Part_Id] = @Part_Id
                      WHERE ([Id] = @Id)"
            );
            
        }
        
        public override void Down()
        {
            AddColumn("dbo.Documents", "IsCheckedOut", c => c.Boolean(nullable: false));
            throw new NotSupportedException("Scaffolding create or alter procedure operations is not supported in down methods.");
        }
    }
}
