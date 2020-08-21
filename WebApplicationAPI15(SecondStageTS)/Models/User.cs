using System;
using System.Collections.Generic;

namespace MessageBoard.Models
{
	public class User
	{
		public Guid Id { get; set; } = Guid.NewGuid();
		public string Name { get; set; }
		public bool IsDeleted { get; set; } = false;
		public List<Announcement> Announcements { get; }
	}
}
