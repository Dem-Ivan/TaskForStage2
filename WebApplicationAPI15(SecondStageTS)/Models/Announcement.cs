using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MessageBoard.Models
{
	public class Announcement
	{
		public Guid Id { get; set; } = Guid.NewGuid();

		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int OrderNumber { get; set; }

		public Guid UserId { get; set; }

		public User user { get; set; }

		public string Text { get; set; }

		public string Image { get; set; }

		public int Rating { get; set; }

		public DateTime CreationDate { get; set; } = DateTime.Now;
		public bool IsDeleted { get; set; } = false;
	}
}
