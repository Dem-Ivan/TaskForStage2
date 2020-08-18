using System;

namespace MessageBoard.dto
{
	public class AnnouncementRespons
	{
		public int OrderNumber { get; set; }
		public UserDTO userDTO { get; set; }
		public string Text { get; set; }
		public string Image { get; set; }
		public int Rating { get; set; }
		public DateTime CreationDate { get; set; }
	}
}
