using MessageBoard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
