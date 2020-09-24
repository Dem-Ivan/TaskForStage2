using AutoMapper;
using MessageBoard.dto;
using MessageBoard.Models;

namespace MessageBoard.Helpers
{
	public class AutoMapperProfile : Profile
	{
		public AutoMapperProfile()
		{
			CreateMap<Announcement, AnnouncementRespons>();
			CreateMap<User, UserDto>();
			CreateMap<UserDto, User>().ForAllMembers(n => n.Condition((src, dest, srcMember) => srcMember != null));
			CreateMap<Announcement, AnnouncementRespons>().ForMember(x => x.UserDto, o => o.MapFrom(s => s.User));
			CreateMap<AnnouncementRequest, Announcement>().ForAllMembers(n => n.Condition((src, dest, srcMember) => srcMember != null));
		}
	}
}
