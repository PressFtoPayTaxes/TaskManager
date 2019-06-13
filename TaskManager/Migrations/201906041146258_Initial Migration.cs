namespace TaskManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DirectoryReplacingTasks",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        PrimordialFolder = c.String(),
                        DestinationFolder = c.String(),
                        Task_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Tasks", t => t.Task_Id)
                .Index(t => t.Task_Id);
            
            CreateTable(
                "dbo.Tasks",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Period = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.EmailSendingTasks",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        MailReceiver = c.String(),
                        MailTheme = c.String(),
                        MailText = c.String(),
                        Task_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Tasks", t => t.Task_Id)
                .Index(t => t.Task_Id);
            
            CreateTable(
                "dbo.FileDownloadingTasks",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        FileUrl = c.String(),
                        Task_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Tasks", t => t.Task_Id)
                .Index(t => t.Task_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FileDownloadingTasks", "Task_Id", "dbo.Tasks");
            DropForeignKey("dbo.EmailSendingTasks", "Task_Id", "dbo.Tasks");
            DropForeignKey("dbo.DirectoryReplacingTasks", "Task_Id", "dbo.Tasks");
            DropIndex("dbo.FileDownloadingTasks", new[] { "Task_Id" });
            DropIndex("dbo.EmailSendingTasks", new[] { "Task_Id" });
            DropIndex("dbo.DirectoryReplacingTasks", new[] { "Task_Id" });
            DropTable("dbo.FileDownloadingTasks");
            DropTable("dbo.EmailSendingTasks");
            DropTable("dbo.Tasks");
            DropTable("dbo.DirectoryReplacingTasks");
        }
    }
}
