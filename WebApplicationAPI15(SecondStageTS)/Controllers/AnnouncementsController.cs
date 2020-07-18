using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;


using WebApplicationAPI15_SecondStageTS_.Context;
using WebApplicationAPI15_SecondStageTS_.dto;
using WebApplicationAPI15_SecondStageTS_.Models;
using WebApplicationAPI15_SecondStageTS_.Options;
using WebApplicationAPI15_SecondStageTS_.ProgectExeptions;
using WebApplicationAPI15_SecondStageTS_.Services.RecaptchaService;
using WebApplicationAPI15_SecondStageTS_.utils;
using WebApplicationAPI15_SecondStageTS_.utils.Paging;


namespace WebApplicationAPI15_SecondStageTS_.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AnnouncementsController : ControllerBase
	{
		private readonly ApplicationContext _context;
		private readonly IMapper _mapper;
		private readonly IRecaptchaService _recaptcha;
		private readonly IOptions<UserOptions> _userOptions;
		private ILogger _logger;
		ObjectState _objecState = new ObjectState();

		public AnnouncementsController(ApplicationContext context, IMapper mapper, IRecaptchaService recaptcha, 
			IOptions<UserOptions> userOptions, ILogger<AnnouncementsController> logger)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
			_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
			_recaptcha = recaptcha ?? throw new ArgumentNullException(nameof(recaptcha));
			_userOptions = userOptions ?? throw new ArgumentNullException(nameof(userOptions));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));			
		}


		//GET api/announcements/1/5   
		[HttpGet("{page}/{pageSize}")]
		public async Task<ActionResult<GetResult<AnnouncementDTOtoFront>>> GetAnnouncements([FromQuery] QueryData queryData, int page = 1, int pageSize = 25)
		{
			if (!ModelState.IsValid) return BadRequest();

			IQueryable<Announcement> announcements = _context.Announcements.Where(an => an.IsDeleted == false);
			
			if (!string.IsNullOrEmpty(queryData.searchString))
			{				
				announcements = announcements.SearchForMatches(queryData.searchString);
			}

			if (queryData.FilterByUserId != null)//если передан userId - проводим фильтрацию по userId
			{
				announcements = announcements.Where(w => w.UserId == queryData.FilterByUserId && w.user.IsDeleted == false);
			}

			announcements = queryData.sortName switch
			{
				"OrderNumber" => announcements.GetSortBy(x => x.OrderNumber, queryData.sortDirection),
				"Rating" => announcements.GetSortBy(x => x.Rating, queryData.sortDirection),
				_ => announcements.GetSortBy(x => x.CreationDate, queryData.sortDirection)
			};

			var pagedResult = announcements.GetPaged(page, pageSize);

			GetResult<AnnouncementDTOtoFront> result = new GetResult<AnnouncementDTOtoFront>();
			result.Rows = await pagedResult.Result.MappingTo<AnnouncementDTOtoFront>(_mapper);
			result.CountRowsFound = pagedResult.RowCount;

			return Ok(result);
		}


		//GET api/announcements/5     
		[HttpGet]
		public async Task<ActionResult<AnnouncementDTOtoFront>> GetAnnouncement(Guid announcementId)
		{
			if (!ModelState.IsValid) return BadRequest();

			var query = _context.Announcements.Where(an => an.Id == announcementId && an.IsDeleted == false);
			var announcementDTO = await _mapper.ProjectTo<AnnouncementDTOtoFront>(query).SingleOrDefaultAsync();

			if (announcementDTO == null) return NotFound();

			return Ok(announcementDTO);
		}

		//POST api/announcements
		[HttpPost]
		public async Task<ActionResult<Guid>> AddAnnouncement([FromBody]AnnouncementDTOtoBack announcementDTO, [FromQuery]Guid userId)
		{

			using (var transaction = _context.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
			{
				try
				{
					var captchaResponse = await _recaptcha.Validate(Request.Form);
					if (!captchaResponse.Success)
					{
						ModelState.AddModelError("reCaptchaError", "reCAPTCHA error occured. Please try again.");
						return RedirectToAction(nameof(Index));
					}
					if (!ModelState.IsValid) return BadRequest();

					var user = await _context.Users.Where(x => x.Id == userId).Include(an => an.Announcements).FirstOrDefaultAsync();

					if (_objecState.UserNotFound(user)) return NotFound("User not found");

					if (user.Announcements.Count >= _userOptions.Value.MaxAnnouncementCount)
					{
						throw new MaxAnnouncementCountException($"Превышено максимальное колличество объяалений!");
					}
					var announcement = _mapper.Map<Announcement>(announcementDTO);
					announcement.user = user;

					_context.Announcements.Add(announcement);
					await _context.SaveChangesAsync();

					transaction.Commit();
					return StatusCode(201, announcement.Id);
				}
				catch (MaxAnnouncementCountException e)
				{
					transaction.Rollback();
					_logger.Log(LogLevel.Warning, "Some Exception in AddAnnouncement() {0}", e);
					return BadRequest(new { e.Message, e.StackTrace });
				}
				catch (Exception e)
				{
					transaction.Rollback();
					_logger.Log(LogLevel.Warning, "Some Exception in AddAnnouncement() {0}", e);
					return StatusCode(500);
				}
			}

		}

		//PUT api/announcements
		[HttpPut]
		public async Task<ActionResult<Guid>> UpdateAnnouncement([FromBody]AnnouncementDTOtoBack announcementDTO, [FromQuery]Guid announcementId)
		{
			if (!ModelState.IsValid) return BadRequest();

			Announcement announcement = await _context.Announcements.Include(u => u.user).SingleOrDefaultAsync(an => an.Id == announcementId);

			if (_objecState.AnnouncementNotFound(announcement)) return NotFound();

			_mapper.Map(announcementDTO, announcement);
			_context.Update(announcement);
			await _context.SaveChangesAsync();

			return Ok(announcement.Id);
		}

		//DELETE api/announcements/5
		[HttpDelete]
		public async Task<ActionResult> DeleteAnnouncement([FromQuery]Guid announcementId)
		{
			if (!ModelState.IsValid) return BadRequest();

			Announcement announcement = await _context.Announcements.SingleOrDefaultAsync(an => an.Id == announcementId);

			if (_objecState.AnnouncementNotFound(announcement)) return NotFound();

			announcement.IsDeleted = true;
			await _context.SaveChangesAsync();

			return StatusCode(204);
		}


	}
}