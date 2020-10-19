using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MessageBoard.dto;
using MessageBoard.utils;
using MessageBoard.Repositoryes;
using MessageBoard.ProgectExceptions;
using System.Threading;

namespace MessageBoard.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UsersController : ControllerBase
	{		
		
		private readonly IRepository<UserDto, UserDto, UserDto> _repository;

		public UsersController(IRepository<UserDto, UserDto, UserDto> repository)
		{
			_repository = repository ?? throw new ArgumentNullException(nameof(repository));
		}

		// GET: api/Users
		[HttpGet("{page}/{pageSize}")]
		public async Task<ActionResult<GetResult<UserDto>>> GetUsers([FromQuery] QueryData queryData, int page = 1, int pageSize = 25, CancellationToken cancellationToken = default)
		{
			if (!ModelState.IsValid) return BadRequest();

			return Ok(await _repository.GetObjectList(queryData, page, pageSize, cancellationToken));

		}

		// GET: api/Users/5
		[HttpGet("{userId}")]
		public async Task<ActionResult<UserDto>> GetUserById(Guid userId, CancellationToken cancellationToken = default)
		{
			if (!ModelState.IsValid) return BadRequest();
			try
			{
				return await _repository.GetObject(userId, cancellationToken);
			}
			catch (ObjectNotFoundException)
			{
				return NotFound();
			}
		}

		[HttpPost]
		public async Task<ActionResult<Guid>> AddUser(UserDto userDTO, CancellationToken cancellationToken = default)
		{
			if (!ModelState.IsValid) return BadRequest();			
			return StatusCode(201, await _repository.CreateObject(userDTO, cancellationToken));
		}

		[HttpPut("{userId}")]
		public async Task<IActionResult> UpdateUser([FromBody]UserDto userDTO, Guid userId, CancellationToken cancellationToken = default)
		{
			if (!ModelState.IsValid) return BadRequest();
			try
			{
				return Ok(await _repository.UpdateObject(userDTO, userId, cancellationToken));
			}
			catch (ObjectNotFoundException)
			{
				return NotFound();
			}			
		}

		// DELETE: api/Users/5
		[HttpDelete("{userId}")]
		public async Task<ActionResult> DeleteUser(Guid userId, CancellationToken cancellationToken = default)
		{
			if (!ModelState.IsValid) return BadRequest();
			try
			{
				return StatusCode(204, await _repository.DeleteObject(userId, cancellationToken));
			}
			catch (ObjectNotFoundException)
			{
				return NotFound();
			}			
		}
	}
}
