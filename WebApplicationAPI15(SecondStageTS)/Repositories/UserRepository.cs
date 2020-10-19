using AutoMapper;
using MessageBoard.Context;
using MessageBoard.dto;
using MessageBoard.Models;
using MessageBoard.ProgectExceptions;
using MessageBoard.utils;
using MessageBoard.utils.Paging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace MessageBoard.Repositoryes
{
	public class UserRepository : IRepository<UserDto, UserDto, UserDto>
	{
		private readonly IMapper _mapper;
		private readonly ApplicationContext _context;

		public UserRepository(IMapper mapper, ApplicationContext context)
		{
			_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}

		public async Task<GetResult<UserDto>> GetObjectList(QueryData queryData, int page, int pageSize, CancellationToken cancellationToken)
		{
			IQueryable<User> usersQuery = _context.Set<User>().Where(u => u.IsDeleted == false).GetSortBy(u => u.Name, queryData.SortDirection);

			PagedResult<UserDto> pagedResult = await usersQuery.GetPaged<UserDto, User>(page, pageSize, _mapper, cancellationToken);

			GetResult<UserDto> result = new GetResult<UserDto>
			{
				Data = pagedResult.Result,
				Count = pagedResult.RowCount
			};

			return result;
		}
		public async Task<UserDto> GetObject(Guid id, CancellationToken cancellationToken)
		{
			IQueryable<User> query = _context.Set<User>().Where(u => u.Id == id && u.IsDeleted == false);

			var userDto = await _mapper.ProjectTo<UserDto>(query).SingleOrDefaultAsync(cancellationToken);
			if (userDto == null) throw new ObjectNotFoundException();

			return userDto;
		}
		public async Task<Guid> CreateObject(UserDto item, CancellationToken cancellationToken)
		{
			var user = _mapper.Map<User>(item);

			await _context.AddAsync(user, cancellationToken);
			await _context.SaveChangesAsync(cancellationToken);

			return user.Id;
		}

		public async Task<Guid> UpdateObject(UserDto item, Guid id, CancellationToken cancellationToken)
		{
			User user = await _context.Set<User>().SingleOrDefaultAsync(u => u.Id == id, cancellationToken);

			if (user.NotFound()) throw new ObjectNotFoundException();

			_mapper.Map(item, user);
			_context.Update(user);
			await _context.SaveChangesAsync(cancellationToken);

			return user.Id;
		}

		public async Task<Guid> DeleteObject(Guid id, CancellationToken  cancellationToken)
		{
			var user = await _context.Set<User>().Include(an => an.Announcements).SingleOrDefaultAsync(u => u.Id == id, cancellationToken);

			if (user.NotFound()) throw new ObjectNotFoundException();

			user.IsDeleted = true;
			foreach (var announcement in user.Announcements)
			{
				announcement.IsDeleted = true;
			}

			await _context.SaveChangesAsync(cancellationToken);
			return user.Id;
		}
	}
}
