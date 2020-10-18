using System;
using System.ComponentModel.DataAnnotations;

namespace MessageBoard.dto
{
	public class AddAnntRequest
	{
		[Required(ErrorMessage = "Не указано значение поля - Text ")]
		[StringLength(100, MinimumLength = 5, ErrorMessage = "Длина строки должна быть от 5 до 100 символов")]
		public string Text { get; set; }
		[Required(ErrorMessage = "Не указано значение поля - Image")]
		public string Image { get; set; }
		[Required(ErrorMessage = "Не указано значение поля - Rating ")]
		[Range(0, 10, ErrorMessage = "Значение поля Рейтинг ограничено диапазоном 0-10")]
		public int Rating { get; set; }
		public Guid UserId { get; set; }

	}
}
