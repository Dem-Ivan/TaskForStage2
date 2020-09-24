using System.ComponentModel.DataAnnotations;

namespace MessageBoard.dto
{
	public class UserDto
	{
		[Required(ErrorMessage = "Укажите имя пользователя")]
		[StringLength(100, MinimumLength = 2, ErrorMessage = "Длина имени должна быть от 2 до 15 символов")]
		public string Name { get; set; }
	}
}
