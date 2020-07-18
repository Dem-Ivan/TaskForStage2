using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplicationAPI15_SecondStageTS_.Models;

namespace WebApplicationAPI15_SecondStageTS_.utils
{
	public class ObjectState
	{

		public bool UserNotFound<T>(T entity) where T : User
		{
			if (entity == null || entity.IsDeleted == true)
			{ return true; }
			else
			{ return false; }
		}

		public bool AnnouncementNotFound<T>(T entity) where T : Announcement
		{
			if (entity == null || entity.IsDeleted == true)
			{ return true; }
			else
			{ return false; }
		}
	}
}
