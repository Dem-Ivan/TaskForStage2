using System;
using System.Linq;
using WebApplicationAPI15_SecondStageTS_.Context;
using WebApplicationAPI15_SecondStageTS_.Models;

namespace WebApplicationAPI15_SecondStageTS_.Services
{
	public static class SimpleData 
	{
		public static void Initialize(ApplicationContext context) 
		{
			if (!context.Users.Any())
			{
				User jon = new User { Name = "Jon"};
				User sem = new User { Name = "Sem" };
				User allis = new User { Name = "Allis"};
				context.Users.AddRange(jon,sem,allis);

				context.Announcements.AddRange(
					new Announcement
					{						
						user = jon,	
						Text = "Продам Трактор",
						Image = "1.png",
						Rating = 1						
					},
					new Announcement
					{						
						user = jon,						
						Text = "Продам велосипед",
						Image = "2.png",
						Rating = 2						
					},
					new Announcement
					{
						user = jon,						
						Text = "Продам квартиру",
						Image = "3.png",
						Rating = 3						
					},
					new Announcement
					{						
						user = sem,						
						Text = "Куплю трактор",
						Image = "2.png",
						Rating = 4						
					},
					new Announcement
					{						
						user = allis,						
						Text = "Учу английскому",
						Image = "1.png",
						Rating = 5						
					});
				context.SaveChanges();
			}
		}
	}
}
