using System.Collections.Generic;
using AutoMapper;
using WebApplicationAPI15_SecondStageTS_.dto;
using WebApplicationAPI15_SecondStageTS_.Models;

namespace WebApplicationAPI15_SecondStageTS_.Helpers
{
	public class AutoMapperProfile : Profile
	{
		public AutoMapperProfile()
		{

			CreateMap<Announcement, AnnouncementDTOtoFront>();
			CreateMap<Announcement, IEnumerable<AnnouncementDTOtoFront>>();
			CreateMap<User, UserDTO>();			
			CreateMap<UserDTO, User>().ForAllMembers(n => n.Condition((src, dest, srcMember) => srcMember != null));
			CreateMap<Announcement, AnnouncementDTOtoFront>().ForMember(x => x.userDTO, o => o.MapFrom(s => s.user));			
			CreateMap<AnnouncementDTOtoBack, Announcement>().ForAllMembers(n => n.Condition((src, dest, srcMember)=> srcMember != null));
			
						
		}
	}
}
