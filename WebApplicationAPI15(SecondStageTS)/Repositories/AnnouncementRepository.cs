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
	public class AnnouncementRepository : IRepository<AnnouncementResponse, AddAnntRequest, UpdateAnntRequest>
	{
		private readonly ApplicationContext _context;	
		private readonly IMapper _mapper;
		private readonly IOptions<UserOptions> _userOptions;
		private readonly ILogger _logger;

		public AnnouncementRepository(IMapper mapper, IOptions<UserOptions> userOptions, ApplicationContext context, ILogger<AnnouncementRepository> logger)
		{
			_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
			_context = context ?? throw new ArgumentNullException(nameof(context));			
			_userOptions = userOptions ?? throw new ArgumentNullException(nameof(userOptions));																			  
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}
		public async Task<GetResult<AnnouncementResponse>> GetObjectList(QueryData queryData, int page, int pageSize, CancellationToken cancellationToken)
		{
			IQueryable<Announcement> announcementsQuery = _context.Set<Announcement>().Where(an => an.IsDeleted == false);
			
			if (!string.IsNullOrEmpty(queryData.SearchString))
			{				
				announcementsQuery = announcementsQuery.SearchForMatches(queryData.SearchString);
			}

			if (queryData.FilterByUserId != null)
			{
				announcementsQuery = announcementsQuery.Where(w => w.UserId == queryData.FilterByUserId && w.User.IsDeleted == false);
			}

			if ("ORDERNUMBER".StartsWith(queryData.SortName.ToUpper()))
			{
				announcementsQuery = announcementsQuery.GetSortBy(x => x.OrderNumber, queryData.SortDirection);
			}
			else if ("RATING".StartsWith(queryData.SortName.ToUpper()))
			{
				announcementsQuery = announcementsQuery.GetSortBy(x => x.Rating, queryData.SortDirection);
			}
			else announcementsQuery = announcementsQuery.GetSortBy(x => x.CreationDate, queryData.SortDirection);
			
			PagedResult<AnnouncementResponse> pagedResult = await announcementsQuery.GetPaged<AnnouncementResponse, Announcement>(page, pageSize, _mapper, cancellationToken);
			GetResult<AnnouncementResponse> result = new GetResult<AnnouncementResponse>
			{
				Data = pagedResult.Result,
				Count = pagedResult.RowCount
			};			

			return result;
		}
		public async Task<AnnouncementResponse> GetObject(Guid id, CancellationToken cancellationToken)
		{					
			var query = _context.Set<Announcement>().Where(an => an.Id == id && an.IsDeleted == false);
			var announcementDto = await _mapper.ProjectTo<AnnouncementResponse>(query).SingleOrDefaultAsync(cancellationToken);			

			if (announcementDto == null) throw new ObjectNotFoundException();

			return announcementDto;
		}
		public async Task<Guid> CreateObject(AddAnntRequest item,  CancellationToken cancellationToken)
		{
			var user = await _context.Set<User>().Where(x => x.Id == item.UserId).FirstOrDefaultAsync(cancellationToken);
			Announcement announcement = null;
			if (user.NotFound()) throw new ObjectNotFoundException();

			using (var transaction = _context.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
			{
				try
				{
					if (_context.Set<Announcement>().Where(an => an.UserId == item.UserId && !an.IsDeleted).Count() >= _userOptions.Value.MaxAnnouncementCount)
					{
						throw new MaxAnnouncementCountException($"Превышено максимальное колличество объяалений!");
					}
					announcement = _mapper.Map<Announcement>(item);
					announcement.User = user;

					await _context.AddAsync(announcement, cancellationToken);

					await transaction.CommitAsync(cancellationToken);
					await _context.SaveChangesAsync(cancellationToken);					
				}				
				catch (MaxAnnouncementCountException e)
				{
					await transaction.RollbackAsync(cancellationToken);
					_logger.Log(LogLevel.Warning, "Some Exception in AddAnnouncement() {0}", e);					
				}
				catch (Exception e)
				{
					await transaction.RollbackAsync(cancellationToken);
					_logger.Log(LogLevel.Warning, "Some Exception in AddAnnouncement() {0}", e);					
				}							
			}
			return announcement.Id;
		}
		public async Task<Guid> UpdateObject(UpdateAnntRequest item, Guid id, CancellationToken cancellationToken)
		{
			Announcement announcement = await _context.Set<Announcement>().Include(u => u.User).SingleOrDefaultAsync(an => an.Id == id);

			if (announcement.NotFound()) throw new ObjectNotFoundException();

			_mapper.Map(item, announcement);
			_context.Update(announcement);			
			await _context.SaveChangesAsync(cancellationToken);

			return announcement.Id;
		}
		public async Task<Guid> DeleteObject(Guid id, CancellationToken cancellationToken)
		{		
			Announcement announcement = await _context.Set<Announcement>().SingleOrDefaultAsync(an => an.Id == id, cancellationToken);

			if (announcement.NotFound()) throw new ObjectNotFoundException();

			announcement.IsDeleted = true;
			await _context.SaveChangesAsync(cancellationToken);
			return announcement.Id;
		}			
	}
}
