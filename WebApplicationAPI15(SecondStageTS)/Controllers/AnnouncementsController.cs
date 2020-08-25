using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using MessageBoard.dto;
using MessageBoard.ProgectExeptions;
using MessageBoard.Services.RecaptchaService;
using MessageBoard.utils;
using MessageBoard.Repositoryes;
using MessageBoard.ProgectExceptions;

namespace MessageBoard.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AnnouncementsController : ControllerBase
	{
		private readonly IRecaptchaService _recaptcha;		
		private IRepository<AnnouncementRespons, AnnouncementRequest> _db;
		

		public AnnouncementsController( IRecaptchaService recaptcha, IRepository<AnnouncementRespons, AnnouncementRequest> db)
		{
			_recaptcha = recaptcha ?? throw new ArgumentNullException(nameof(recaptcha));
			_db = db ?? throw new ArgumentNullException(nameof(db));
		}


		//GET api/announcements/1/5   
		[HttpGet("{page}/{pageSize}")]
		public async Task<ActionResult<GetResult<AnnouncementRespons>>> GetAnnouncements([FromQuery] QueryData queryData, int page = 1, int pageSize = 25)
		{
			if (!ModelState.IsValid) return BadRequest();
			try
			{
				return Ok(_db.GetList(queryData, page, pageSize));
			}
			catch (ObjectNotFoundException)
			{
				return NotFound();
			}			
		}


		//GET api/announcements/5     
		[HttpGet]
		public async Task<ActionResult<AnnouncementRespons>> GetAnnouncement(Guid announcementId)
		{
			if (!ModelState.IsValid) return BadRequest();

			try
			{				
				var ann = _db.Get(announcementId);
				return Ok(ann);
			}
			catch (ObjectNotFoundException)
			{
				return NotFound();
			}	
		}

		//POST api/announcements
		[HttpPost]
		public async Task<ActionResult<Guid>> AddAnnouncement([FromBody]AnnouncementRequest announcementDTO, [FromQuery]Guid userId)
		{
			var captchaResponse = await _recaptcha.Validate(Request.Form);
			if (!captchaResponse.Success) throw new ReCaptchaErrorException("Не улалось пройти рекапчу, попробуйте снова!");
			if (!ModelState.IsValid) return BadRequest();

			try
			{
				_db.Create(announcementDTO, userId);		
				return StatusCode(201);				
			}
			catch (ReCaptchaErrorException e)
			{
				return BadRequest(new { e.Message });
			}
			catch (MaxAnnouncementCountException e)
			{
				return BadRequest(new { e.Message });
			}
		}

		//PUT api/announcements
		[HttpPut]
		public async Task<ActionResult> UpdateAnnouncement([FromBody]AnnouncementRequest announcementDTO, [FromQuery]Guid announcementId)
		{
			if (!ModelState.IsValid) return BadRequest();
			try
			{
				_db.Update(announcementDTO, announcementId);			
				return Ok();
			}
			catch (ObjectNotFoundException)
			{
				return NotFound();
			}			
		}

		//DELETE api/announcements/5
		[HttpDelete]
		public async Task<ActionResult> DeleteAnnouncement([FromQuery]Guid announcementId)
		{
			if (!ModelState.IsValid) return BadRequest();
			try
			{
				_db.Delete(announcementId);		
				return StatusCode(204);
			}
			catch (ObjectNotFoundException)
			{
				return NotFound();
			}			
		}
	}
}