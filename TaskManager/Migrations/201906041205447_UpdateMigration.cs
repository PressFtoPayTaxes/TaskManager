namespace TaskManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateMigration : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tasks", "Name", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tasks", "Name");
        }
    }
}
