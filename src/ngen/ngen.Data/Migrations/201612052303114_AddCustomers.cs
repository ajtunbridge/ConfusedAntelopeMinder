namespace ngen.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCustomers : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Employees", "PasswordHash", c => c.String(nullable: false, maxLength: 88, fixedLength: true, unicode: false));
            AlterColumn("dbo.Employees", "PasswordSalt", c => c.String(nullable: false, maxLength: 24, fixedLength: true, unicode: false));
            AlterColumn("dbo.DocumentVersions", "Hash", c => c.String(nullable: false, maxLength: 32, fixedLength: true, unicode: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.DocumentVersions", "Hash", c => c.String(nullable: false, maxLength: 32));
            AlterColumn("dbo.Employees", "PasswordSalt", c => c.String(nullable: false, maxLength: 24));
            AlterColumn("dbo.Employees", "PasswordHash", c => c.String(nullable: false, maxLength: 88));
        }
    }
}
