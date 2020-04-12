using Microsoft.EntityFrameworkCore;

namespace WebApplicationAPI15_SecondStageTS_.Models
{
	public class ApplicationContext : DbContext
	{
		public DbSet<User> Users { get; set; }
		public DbSet<Announcement> Announcements { get; set; }
		public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
		{

		}
	}
}
