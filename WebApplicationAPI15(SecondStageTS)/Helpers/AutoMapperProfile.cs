using AutoMapper;
using MessageBoard.dto;
using MessageBoard.Models;

namespace MessageBoard.Helpers
{
	public class AutoMapperProfile : Profile
	{
		public AutoMapperProfile()
		{
			//CreateMap<Announcement, AnnouncementRespons>();
			CreateMap<User, UserDTO>();
			CreateMap<UserDTO, User>().ForAllMembers(n => n.Condition((src, dest, srcMember) => srcMember != null));
			CreateMap<Announcement, AnnouncementRespons>().ForMember(x => x.userDTO, o => o.MapFrom(s => s.user));
			CreateMap<AnnouncementRequest, Announcement>().ForAllMembers(n => n.Condition((src, dest, srcMember) => srcMember != null));
		}
	}
}
