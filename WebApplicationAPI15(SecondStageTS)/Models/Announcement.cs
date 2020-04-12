using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplicationAPI15_SecondStageTS_.Models
{
	public class Announcement
	{
		
		public Guid Id { get; set; } = Guid.NewGuid();

		[Required( ErrorMessage = "Не указано значение поля - OrderNumber ")]
		public int OrderNumber { get; set; }
		
		public Guid UserId { get; set; }

		[Required(ErrorMessage = "Не указано значение поля - User ")]		
		public User user { get; set; }
		
		[Required(ErrorMessage = "Не указано значение поля - Text ")]
		[StringLength(100, MinimumLength = 5, ErrorMessage = "Длина строки должна быть от 5 до 100 символов")]
		public string Text { get; set; }
		
		[Required(ErrorMessage = "Не указано значение поля - Image" )]
		public string Image { get; set; }

		[Required(ErrorMessage = "Не указано значение поля - Rating ")]
		public int Rating { get; set; }

		[Required(ErrorMessage = "Не указано значение поля - CreationDate ")]
		public DateTime CreationDate { get; set; }
	}
}
