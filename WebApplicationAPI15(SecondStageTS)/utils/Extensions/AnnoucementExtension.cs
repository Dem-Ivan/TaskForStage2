using MessageBoard.Models;

namespace MessageBoard.utils
{
	public static class AnnoucementExtension
	{
		public static bool NotFound (this Announcement announcement ) 
		{
			if (announcement == null || announcement.IsDeleted)
			{ return true; }
			else
			{ return false; }
		}
	}
}
