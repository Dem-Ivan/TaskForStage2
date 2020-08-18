using MessageBoard.Models;

namespace MessageBoard.utils
{
	public class ObjectState
	{

		public bool UserNotFound<T>(T entity) where T : User
		{
			if (entity == null || entity.IsDeleted)
			{ return true; }
			else
			{ return false; }
		}

		public bool AnnouncementNotFound<T>(T entity) where T : Announcement
		{
			if (entity == null || entity.IsDeleted)
			{ return true; }
			else
			{ return false; }
		}
	}
}
