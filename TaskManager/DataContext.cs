namespace TaskManager
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using TaskManager.Models1;

    public class DataContext : DbContext
    {
        public DataContext()
            : base("name=DataContext")
        {
        }

        public DbSet<Task> Tasks { get; set; }
        public DbSet<EmailSendingTask> EmailSendingTasks { get; set; }
        public DbSet<FileDownloadingTask> FileDownloadingTasks { get; set; }
        public DbSet<DirectoryReplacingTask> DirectoryReplacingTasks { get; set; }
    }
}