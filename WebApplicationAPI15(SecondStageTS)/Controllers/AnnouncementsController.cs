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
				return Ok(await _db.GetObjectList(queryData, page, pageSize));				
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
				return Ok(await _db.GetObject(announcementId));
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
			//var captchaResponse = await _recaptcha.Validate(Request.Form);
			//if (!captchaResponse.Success) throw new ReCaptchaErrorException("Не удалось пройти рекапчу, попробуйте снова!");
			if (!ModelState.IsValid) return BadRequest();
			try
			{					
				return StatusCode(201, await _db.CreateObject(announcementDTO, userId));				
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
				return Ok(await _db.UpdateObject(announcementDTO, announcementId));
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
				return StatusCode(204, await _db.DeleteObject(announcementId));
			}
			catch (ObjectNotFoundException)
			{
				return NotFound();
			}			
		}
	}
}