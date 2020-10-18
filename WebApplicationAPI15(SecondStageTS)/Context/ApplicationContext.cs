using Microsoft.EntityFrameworkCore;
using MessageBoard.Models;
using System;

namespace MessageBoard.Context
{
	public class ApplicationContext : DbContext
	{

		public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
		{

		}
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			if (modelBuilder == null) throw new ArgumentNullException(nameof(modelBuilder));

			modelBuilder.Entity<User>().ToTable("Users");
			modelBuilder.Entity<Announcement>().ToTable("Announcements"); ;
			
			modelBuilder.Entity<Announcement>()
				.HasOne(u => u.User)
				.WithMany(an => an.Announcements)
				.HasForeignKey(u => u.UserId);
		}
	}
}
