using AutoMapper;
using MessageBoard.dto;
using MessageBoard.Models;

namespace MessageBoard.Helpers
{
	public class AutoMapperProfile : Profile
	{
		public AutoMapperProfile()
		{
			CreateMap<Announcement, AnnouncementResponse>();
			CreateMap<User, UserDto>();
			CreateMap<UserDto, User>().ForAllMembers(n => n.Condition((src, dest, srcMember) => srcMember != null));
			CreateMap<Announcement, AnnouncementResponse>().ForMember(x => x.UserDto, o => o.MapFrom(s => s.User));
			CreateMap<AddAnntRequest, Announcement>().ForAllMembers(n => n.Condition((src, dest, srcMember) => srcMember != null));
		}
	}
}
