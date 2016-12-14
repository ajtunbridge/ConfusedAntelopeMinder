namespace ngen.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenamedHierarchyToLineageOnWCGroup : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.WorkCentreGroups", "Lineage", c => c.String());
            DropColumn("dbo.WorkCentreGroups", "Hierarchy");
        }
        
        public override void Down()
        {
            AddColumn("dbo.WorkCentreGroups", "Hierarchy", c => c.String());
            DropColumn("dbo.WorkCentreGroups", "Lineage");
        }
    }
}
