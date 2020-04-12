using System;
using System.Linq;
using WebApplicationAPI15_SecondStageTS_.Models;

namespace WebApplicationAPI15_SecondStageTS_.Services
{
	public static class SimpleData 
	{
		public static void Initialize(ApplicationContext context) 
		{
			if (!context.Users.Any())
			{
				User jon = new User { Name = "Jon" };
				User sem = new User { Name = "Sem" };
				User allis = new User { Name = "Allis"};
				context.Users.AddRange(jon,sem,allis);

				context.Announcements.AddRange(
					new Announcement
					{
						OrderNumber = 1,
						user = jon,
						UserId = jon.Id,
						Text = "Продам Трактор",
						Image = "1.png",
						Rating = 1,
						CreationDate = DateTime.Now
					},
					new Announcement
					{
						OrderNumber = 2,
						user = jon,
						UserId = jon.Id,
						Text = "Продам велосипед",
						Image = "2.png",
						Rating = 2,
						CreationDate = DateTime.Now
					},
					new Announcement
					{
						OrderNumber = 3,
						user = jon,
						UserId = jon.Id,
						Text = "Продам квартиру",
						Image = "3.png",
						Rating = 3,
						CreationDate = DateTime.Now
					},
					new Announcement
					{
						OrderNumber = 4,
						user = sem,
						UserId = sem.Id,
						Text = "Куплю трактор",
						Image = "2.png",
						Rating = 4,
						CreationDate = DateTime.Now
					},
					new Announcement
					{
						OrderNumber = 5,
						user = allis,
						UserId =allis.Id,
						Text = "Учу английскому",
						Image = "1.png",
						Rating = 5,
						CreationDate = DateTime.Now
					});
				context.SaveChanges();
			}
		}
	}
}
