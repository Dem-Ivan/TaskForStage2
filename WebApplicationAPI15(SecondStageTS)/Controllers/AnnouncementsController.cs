using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using MessageBoard.dto;
using MessageBoard.ProgectExeptions;
using MessageBoard.Services.RecaptchaService;
using MessageBoard.utils;
using MessageBoard.Repositoryes;
using MessageBoard.ProgectExceptions;
using System.Threading;

namespace MessageBoard.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AnnouncementsController : ControllerBase
	{
		private readonly IRecaptchaService _recaptcha;		
		private readonly IRepository<AnnouncementResponse, AddAnntRequest, UpdateAnntRequest> _repository;		

		public AnnouncementsController( IRecaptchaService recaptcha, IRepository<AnnouncementResponse, AddAnntRequest, UpdateAnntRequest> repository)
		{
			_recaptcha = recaptcha ?? throw new ArgumentNullException(nameof(recaptcha));
			_repository = repository ?? throw new ArgumentNullException(nameof(repository));			
		}

		//GET api/announcements/1/5   
		[HttpGet("{page}/{pageSize}")]
		public async Task<ActionResult<GetResult<AnnouncementResponse>>> GetAnnouncements([FromQuery] QueryData queryData, int page = 1, int pageSize = 25, CancellationToken cancellationToken = default)
		{
			if (!ModelState.IsValid) return BadRequest();

			return Ok(await _repository.GetObjectList(queryData, page, pageSize, cancellationToken));	
					
		}


		//GET api/announcements/5     
		[HttpGet("{announcementId}")]
		public async Task<ActionResult<AnnouncementResponse>> GetAnnouncement(Guid announcementId, CancellationToken cancellationToken = default)
		{
			if (!ModelState.IsValid) return BadRequest();
			try
			{						
				return Ok(await _repository.GetObject(announcementId, cancellationToken));
			}
			catch (ObjectNotFoundException)
			{
				return NotFound();
			}	
		}

		//POST api/announcements
		[HttpPost]
		public async Task<ActionResult<Guid>> AddAnnouncement([FromBody]AddAnntRequest announcementRequest,  CancellationToken cancellationToken = default)
		{
			var captchaResponse = await _recaptcha.Validate(Request.Form);
			if (!captchaResponse.Success) throw new ReCaptchaErrorException("Не удалось пройти рекапчу, попробуйте снова!");
			if (!ModelState.IsValid) return BadRequest();
			try
			{					
				return StatusCode(201, await _repository.CreateObject(announcementRequest, cancellationToken));				
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
		[HttpPut("{announcementId}")]
		public async Task<ActionResult> UpdateAnnouncement([FromBody]UpdateAnntRequest updateAnntRequest, Guid announcementId, CancellationToken cancellationToken = default)
		{
			if (!ModelState.IsValid) return BadRequest();
			try
			{							
				return Ok(await _repository.UpdateObject(updateAnntRequest, announcementId, cancellationToken));
			}
			catch (ObjectNotFoundException)
			{
				return NotFound();
			}			
		}

		//DELETE api/announcements/5
		[HttpDelete("{announcementId}")]
		public async Task<ActionResult> DeleteAnnouncement(Guid announcementId, CancellationToken cancellationToken = default)
		{
			if (!ModelState.IsValid) return BadRequest();
			try
			{					
				return StatusCode(204, await _repository.DeleteObject(announcementId, cancellationToken));
			}
			catch (ObjectNotFoundException)
			{
				return NotFound();
			}			
		}
	}
}