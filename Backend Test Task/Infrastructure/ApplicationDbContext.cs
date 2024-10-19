using Backend_Test_Task.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend_Test_Task.Infrastructure
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Tree> Trees { get; set; }
        public DbSet<TreeNode> TreeNodes { get; set; }
        public DbSet<ExceptionJournal> ExceptionJournals { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TreeNode>()
                .HasOne(n => n.Tree)
                .WithMany(t => t.Nodes)
                .HasForeignKey(n => n.TreeId);

            modelBuilder.Entity<TreeNode>()
                .HasOne(n => n.Parent)
                .WithMany(n => n.Children)
                .HasForeignKey(n => n.ParentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

}
