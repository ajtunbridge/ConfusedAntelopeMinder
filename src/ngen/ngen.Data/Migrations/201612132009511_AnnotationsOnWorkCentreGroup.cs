namespace ngen.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AnnotationsOnWorkCentreGroup : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.WorkCentreGroups", "Name", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.WorkCentreGroups", "DefaultHourlyRate", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.WorkCentreGroups", "DefaultHourlyRate", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.WorkCentreGroups", "Name", c => c.String());
        }
    }
}
