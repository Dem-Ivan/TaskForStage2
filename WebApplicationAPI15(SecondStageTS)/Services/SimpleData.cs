using System.Linq;
using MessageBoard.Context;
using MessageBoard.Models;

namespace MessageBoard.Services
{
	public static class SimpleData
	{
		public static void Initialize(ApplicationContext context)
		{
			if (!context.Set<User>().Any())
			{
				User jon = new User { Name = "Jon" };
				User sem = new User { Name = "Sem" };
				User allis = new User { Name = "Allis" };
				context.AddRange(jon, sem, allis);

				context.AddRange(
					new Announcement
					{
						User = jon,
						Text = "Продам Трактор",
						Image = "1.png",
						Rating = 1
					},
					new Announcement
					{
						User = jon,
						Text = "Продам велосипед",
						Image = "2.png",
						Rating = 2
					},
					new Announcement
					{
						User = jon,
						Text = "Продам квартиру",
						Image = "3.png",
						Rating = 3
					},
					new Announcement
					{
						User = sem,
						Text = "Куплю трактор",
						Image = "2.png",
						Rating = 4
					},
					new Announcement
					{
						User = allis,
						Text = "Учу английскому",
						Image = "1.png",
						Rating = 5
					});
				context.SaveChanges();
			}
		}
	}
}
