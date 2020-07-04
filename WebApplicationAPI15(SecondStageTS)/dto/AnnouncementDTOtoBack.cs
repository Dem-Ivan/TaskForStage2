using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplicationAPI15_SecondStageTS_.dto
{
	public class AnnouncementDTOtoBack
	{
		[Required(ErrorMessage = "Не указано значение поля - Text ")]
		[StringLength(100, MinimumLength = 5, ErrorMessage = "Длина строки должна быть от 5 до 100 символов")]
		public string Text { get; set; }
		[Required(ErrorMessage = "Не указано значение поля - Image")]
		public string Image { get; set; }
		[Required(ErrorMessage = "Не указано значение поля - Rating ")]
		public int Rating { get; set; }		
	}
}
