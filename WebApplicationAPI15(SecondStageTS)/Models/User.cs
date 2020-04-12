using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace WebApplicationAPI15_SecondStageTS_.Models
{
	public class User
	{		
		public Guid Id {get; set;}= Guid.NewGuid();

		[Required(ErrorMessage = "Укажите имя пользователя")]
		public string Name { get; set; }
		public List<Announcement> Announcements { get; set; } //= new List<Announcement>();
	}
}
