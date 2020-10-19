using MessageBoard.Models;

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
