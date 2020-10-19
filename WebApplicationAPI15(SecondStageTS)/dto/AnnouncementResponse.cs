using System;

namespace MessageBoard.dto
{
	public class AnnouncementResponse
	{
		public int OrderNumber { get; set; }

		public UserDto UserDto { get; set; }

		public string Text { get; set; }

		public string Image { get; set; }

		public int Rating { get; set; }

		public DateTime CreationDate { get; set; }
	}
}
