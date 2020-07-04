using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplicationAPI15_SecondStageTS_.dto
{
	public class UserDTO
	{
		[Required(ErrorMessage = "Укажите имя пользователя")]
		public string Name { get; set; }
	}
}
