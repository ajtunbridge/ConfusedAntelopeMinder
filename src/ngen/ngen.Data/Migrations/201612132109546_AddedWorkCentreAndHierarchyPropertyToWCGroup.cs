namespace ngen.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedWorkCentreAndHierarchyPropertyToWCGroup : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.WorkCentreGroups", "Hierarchy", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.WorkCentreGroups", "Hierarchy");
        }
    }
}
