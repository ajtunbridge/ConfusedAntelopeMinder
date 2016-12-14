namespace ngen.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedMaxVersionCount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ClientSettings", "MaximumVersionCount", c => c.Byte(nullable: false));
            AlterStoredProcedure(
                "dbo.ClientSetting_Insert",
                p => new
                    {
                        CompanyName = p.String(maxLength: 100),
                        DocumentStorageTechnology = p.String(maxLength: 5),
                        MaximumVersionCount = p.Byte(),
                    },
                body:
                    @"INSERT [dbo].[ClientSettings]([CompanyName], [DocumentStorageTechnology], [MaximumVersionCount])
                      VALUES (@CompanyName, @DocumentStorageTechnology, @MaximumVersionCount)
                      
                      DECLARE @Id int
                      SELECT @Id = [Id]
                      FROM [dbo].[ClientSettings]
                      WHERE @@ROWCOUNT > 0 AND [Id] = scope_identity()
                      
                      SELECT t0.[Id]
                      FROM [dbo].[ClientSettings] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[Id] = @Id"
            );
            
            AlterStoredProcedure(
                "dbo.ClientSetting_Update",
                p => new
                    {
                        Id = p.Int(),
                        CompanyName = p.String(maxLength: 100),
                        DocumentStorageTechnology = p.String(maxLength: 5),
                        MaximumVersionCount = p.Byte(),
                    },
                body:
                    @"UPDATE [dbo].[ClientSettings]
                      SET [CompanyName] = @CompanyName, [DocumentStorageTechnology] = @DocumentStorageTechnology, [MaximumVersionCount] = @MaximumVersionCount
                      WHERE ([Id] = @Id)"
            );
            
        }
        
        public override void Down()
        {
            DropColumn("dbo.ClientSettings", "MaximumVersionCount");
            throw new NotSupportedException("Scaffolding create or alter procedure operations is not supported in down methods.");
        }
    }
}
