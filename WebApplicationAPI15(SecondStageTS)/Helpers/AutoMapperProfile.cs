using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using WebApplicationAPI15_SecondStageTS_.dto;
using WebApplicationAPI15_SecondStageTS_.Models;
using WebApplicationAPI15_SecondStageTS_.Services;

namespace WebApplicationAPI15_SecondStageTS_.Helpers
{
	public class AutoMapperProfile : Profile
	{
		public AutoMapperProfile()
		{
			CreateMap<Announcement, AnnouncementDTO>();
			CreateMap<Announcement, IEnumerable<AnnouncementDTO>>();
			CreateMap<User, UserDTO>();
			CreateMap<UserDTO, User>();
			CreateMap<Announcement,AnnouncementDTO>().ForMember(x => x.user, o => o.MapFrom(s => s.user));
			CreateMap<AnnouncementDTO, Announcement>().ForMember(s => s.user, o => o.MapFrom(x => x.user));
		}
	}
}
