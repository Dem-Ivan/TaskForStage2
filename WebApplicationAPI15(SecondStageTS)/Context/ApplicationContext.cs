using Microsoft.EntityFrameworkCore;
using MessageBoard.Models;
using System;

namespace MessageBoard.Context
{
	public class ApplicationContext : DbContext
	{
		public DbSet<User> Users { get; set; }
		public DbSet<Announcement> Announcements { get; set; }

		
		public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
		{

		}
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			if (modelBuilder == null) throw new ArgumentNullException(nameof(modelBuilder));

			modelBuilder.Entity<Announcement>()
				.HasOne(u => u.user)
				.WithMany(an => an.Announcements)
				.HasForeignKey(u => u.UserId);
		}
	}
}
