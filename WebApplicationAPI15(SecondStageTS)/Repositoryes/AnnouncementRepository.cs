using AutoMapper;
using MessageBoard.Context;
using MessageBoard.dto;
using MessageBoard.Models;
using MessageBoard.Options;
using MessageBoard.ProgectExceptions;
using MessageBoard.ProgectExeptions;
using MessageBoard.Services.RecaptchaService;
using MessageBoard.utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MessageBoard.utils.Paging;

namespace MessageBoard.Repositoryes
{
	public class AnnouncementRepository : IRepository<AnnouncementRespons, AnnouncementRequest>
	{
		private ApplicationContext db;
		private ObjectState _objecState = new ObjectState();//добавить в DI?
		private readonly IMapper _mapper;
		private readonly IOptions<UserOptions> _userOptions;
		private ILogger _logger;

		public AnnouncementRepository(IMapper mapper, IOptions<UserOptions> userOptions, ApplicationContext context, ILogger<AnnouncementRepository> logger)
		{
			this.db = context ?? throw new ArgumentNullException(nameof(context));
			_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
			_userOptions = userOptions ?? throw new ArgumentNullException(nameof(userOptions));																			  
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}
		public async Task<GetResult<AnnouncementRespons>> GetList(QueryData queryData, int page, int pageSize)
		{
			IQueryable<Announcement> announcementsQuery = db.Announcements.Where(an => an.IsDeleted == false);

			if (!string.IsNullOrEmpty(queryData.SearchString))
			{
				announcementsQuery = announcementsQuery.SearchForMatches(queryData.SearchString);
				if (announcementsQuery == null) throw new ObjectNotFoundException();//////////////////////////нужен тестовый прогон!
			}
			if (queryData.FilterByUserId != null)//если передан userId - проводим фильтрацию по userId
			{
				announcementsQuery = announcementsQuery.Where(w => w.UserId == queryData.FilterByUserId && w.user.IsDeleted == false);
			}

			announcementsQuery = queryData.SortName switch
			{
				"OrderNumber" => announcementsQuery.GetSortBy(x => x.OrderNumber, queryData.sortDirection),
				"Rating" => announcementsQuery.GetSortBy(x => x.Rating, queryData.sortDirection),
				_ => announcementsQuery.GetSortBy(x => x.CreationDate, queryData.sortDirection)
			};

			var pagedResult = announcementsQuery.GetPaged(page, pageSize);

			GetResult<AnnouncementRespons> result = new GetResult<AnnouncementRespons>();
			result.Rows = await pagedResult.Result.MappingTo<AnnouncementRespons>(_mapper);
			result.CountRowsFound = pagedResult.RowCount;

			return result;
		}
		public async Task<AnnouncementRespons> Get(Guid Id)
		{
			var query = db.Announcements.Where(an => an.Id == Id && an.IsDeleted == false);
			var announcementDTO = await _mapper.ProjectTo<AnnouncementRespons>(query).SingleOrDefaultAsync();

			if (announcementDTO == null) throw new ObjectNotFoundException();

			return announcementDTO;
		}
		public async void Create(AnnouncementRequest item, Guid Id)
		{
			var user = await db.Users.Where(x => x.Id == Id).FirstOrDefaultAsync();

			if (_objecState.UserNotFound(user)) throw new ObjectNotFoundException();

			using (var transaction = db.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
			{
				try
				{
					if (db.Announcements.Where(an => an.UserId == Id && !an.IsDeleted).Count() >= _userOptions.Value.MaxAnnouncementCount)
					{
						throw new MaxAnnouncementCountException($"Превышено максимальное колличество объяалений!");
					}
					var announcement = _mapper.Map<Announcement>(item);
					announcement.user = user;

					db.Announcements.Add(announcement);

					transaction.Commit();
					await db.SaveChangesAsync();					
				}				
				catch (MaxAnnouncementCountException e)
				{
					transaction.Rollback();
					_logger.Log(LogLevel.Warning, "Some Exception in AddAnnouncement() {0}", e);					
				}
				catch (Exception e)
				{
					transaction.Rollback();
					_logger.Log(LogLevel.Warning, "Some Exception in AddAnnouncement() {0}", e);					
				}
							
			}
		}
		public async void Update(AnnouncementRequest item, Guid Id)
		{
			Announcement announcement = await db.Announcements.Include(u => u.user).SingleOrDefaultAsync(an => an.Id == Id);

			if (_objecState.AnnouncementNotFound(announcement)) throw new ObjectNotFoundException();

			_mapper.Map(item, announcement);
			
			db.Update(announcement);
			await db.SaveChangesAsync();
		}
		public async void  Delete(Guid Id)
		{		
			Announcement announcement = await db.Announcements.SingleOrDefaultAsync(an => an.Id == Id);

			if (_objecState.AnnouncementNotFound(announcement)) throw new ObjectNotFoundException();

			announcement.IsDeleted = true;
			await db.SaveChangesAsync();
		}			
	}
}
