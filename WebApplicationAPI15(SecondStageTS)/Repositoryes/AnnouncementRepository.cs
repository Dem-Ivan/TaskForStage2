using AutoMapper;
using MessageBoard.Context;
using MessageBoard.dto;
using MessageBoard.Models;
using MessageBoard.Options;
using MessageBoard.ProgectExceptions;
using MessageBoard.ProgectExeptions;
using MessageBoard.utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MessageBoard.utils.Paging;
using System.Threading;

namespace MessageBoard.Repositoryes
{
	public class AnnouncementRepository : IRepository<AnnouncementRespons, AnnouncementRequest>
	{
		private ApplicationContext _context;
	
		private readonly IMapper _mapper;
		private readonly IOptions<UserOptions> _userOptions;
		private ILogger _logger;

		public AnnouncementRepository(IMapper mapper, IOptions<UserOptions> userOptions, ApplicationContext context, ILogger<AnnouncementRepository> logger)
		{
			_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
			_context = context ?? throw new ArgumentNullException(nameof(context));			
			_userOptions = userOptions ?? throw new ArgumentNullException(nameof(userOptions));																			  
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}
		public async Task<GetResult<AnnouncementRespons>> GetObjectList(QueryData queryData, int page, int pageSize, CancellationToken cancellationToken)
		{
			IQueryable<Announcement> announcementsQuery = _context.Announcements.Where(an => an.IsDeleted == false);
			
			if (!string.IsNullOrEmpty(queryData.SearchString))
			{
				announcementsQuery = announcementsQuery.SearchForMatches(queryData.SearchString);
			}
			if (queryData.FilterByUserId != null)//если передан userId - проводим фильтрацию по userId
			{
				announcementsQuery = announcementsQuery.Where(w => w.UserId == queryData.FilterByUserId && w.User.IsDeleted == false);
			}

			if ("ORDERNUMBER".StartsWith(queryData.SortName.ToUpper()))
			{
				announcementsQuery = announcementsQuery.GetSortBy(x => x.OrderNumber, queryData.sortDirection);
			}
			else if ("RATING".StartsWith(queryData.SortName.ToUpper()))
			{
				announcementsQuery = announcementsQuery.GetSortBy(x => x.Rating, queryData.sortDirection);
			}
			else announcementsQuery = announcementsQuery.GetSortBy(x => x.CreationDate, queryData.sortDirection);
			
			PagedResult<AnnouncementRespons> pagedResult = await announcementsQuery.GetPaged<AnnouncementRespons, Announcement>(page, pageSize, _mapper, cancellationToken);
			GetResult<AnnouncementRespons> result = new GetResult<AnnouncementRespons>
			{
				Data = pagedResult.Result,
				Count = pagedResult.RowCount
			};			

			return result;
		}
		public async Task<AnnouncementRespons> GetObject(Guid Id, CancellationToken cancellationToken)
		{					
			var query = _context.Announcements.Where(an => an.Id == Id && an.IsDeleted == false);
			var announcementDTO = await _mapper.ProjectTo<AnnouncementRespons>(query).SingleOrDefaultAsync(cancellationToken);
			

			if (announcementDTO == null) throw new ObjectNotFoundException();

			return announcementDTO;
		}
		public async Task<Guid> CreateObject(AnnouncementRequest item, Guid Id, CancellationToken cancellationToken)
		{
			var user = await _context.Users.Where(x => x.Id == Id).FirstOrDefaultAsync(cancellationToken);
			Announcement announcement = null;
			if (user.NotFound()) throw new ObjectNotFoundException();

			using (var transaction = _context.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
			{
				try
				{
					if (_context.Announcements.Where(an => an.UserId == Id && !an.IsDeleted).Count() >= _userOptions.Value.MaxAnnouncementCount)
					{
						throw new MaxAnnouncementCountException($"Превышено максимальное колличество объяалений!");
					}
					announcement = _mapper.Map<Announcement>(item);
					announcement.User = user;

					//db.Announcements.Add(announcement);
					await _context.Announcements.AddAsync(announcement, cancellationToken);

					transaction.Commit();
					await _context.SaveChangesAsync(cancellationToken);					
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
			return (announcement.Id);
		}
		public async Task<Guid> UpdateObject(AnnouncementRequest item, Guid Id, CancellationToken cancellationToken)
		{
			Announcement announcement = await _context.Announcements.Include(u => u.User).SingleOrDefaultAsync(an => an.Id == Id);

			if (announcement.NotFound()) throw new ObjectNotFoundException();

			_mapper.Map(item, announcement);
			_context.Update(announcement);
			await _context.SaveChangesAsync(cancellationToken);

			return announcement.Id;
		}
		public async Task<Guid> DeleteObject(Guid Id, CancellationToken cancellationToken)
		{		
			Announcement announcement = await _context.Announcements.SingleOrDefaultAsync(an => an.Id == Id, cancellationToken);

			if (announcement.NotFound()) throw new ObjectNotFoundException();

			announcement.IsDeleted = true;
			await _context.SaveChangesAsync(cancellationToken);
			return (announcement.Id);
		}			
	}
}
