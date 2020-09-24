using MessageBoard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessageBoard.utils
{
	public static class UserExtension
	{
		public static bool NotFound(this User user)
		{
			if (user == null || user.IsDeleted)
			{ return true; }
			else
			{ return false; }
		}
	}
}
