using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplicationAPI15_SecondStageTS_.dto
{
	public class AnnouncementDTOtoFront
	{
		public int OrderNumber { get; set; }
		public UserDTO userDTO { get; set; }
		public string Text { get; set; }
		public string Image { get; set; }
		public int Rating { get; set; }
		public DateTime CreationDate { get; set; }
	}
}
