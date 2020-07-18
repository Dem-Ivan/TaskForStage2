using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplicationAPI15_SecondStageTS_.dto
{
	public class UserDTO
	{
		[Required(ErrorMessage = "Укажите имя пользователя")]
		[StringLength(100, MinimumLength = 2, ErrorMessage = "Длина имени должна быть от 2 до 15 символов")]
		public string Name { get; set; }
	}
}
