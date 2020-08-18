using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MessageBoard.Context;
using MessageBoard.dto;
using MessageBoard.Models;
using MessageBoard.utils;
using MessageBoard.utils.Paging;

namespace MessageBoard.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UsersController : ControllerBase
	{
		private readonly ApplicationContext _context;
		private readonly IMapper _mapper;
		ObjectState _objecState = new ObjectState();

		public UsersController(ApplicationContext context, IMapper mapper)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
			_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
		}

		// GET: api/Users
		[HttpGet]
		public async Task<ActionResult<GetResult<UserDTO>>> GetUsers([FromQuery] QueryData queryData, int page = 1, int pageSize = 25)
		{
			if (!ModelState.IsValid) return BadRequest();

			IQueryable<User> users = _context.Users;
			users = users.Where(u => u.IsDeleted == false).GetSortBy(u => u.Name, queryData.sortDirection);

			if (users == null) return NotFound();
			var pagedResult = users.GetPaged(page, pageSize);

			GetResult<UserDTO> result = new GetResult<UserDTO>();
			result.Rows = await pagedResult.Result.MappingTo<UserDTO>(_mapper);
			result.CountRowsFound = pagedResult.RowCount;

			return Ok(result);
		}

		// GET: api/Users/5
		[HttpGet("{id}")]
		public async Task<ActionResult<UserDTO>> GetUser(Guid userId)
		{

			if (!ModelState.IsValid) return BadRequest();

			IQueryable<User> query = _context.Users.Where(u => u.Id == userId && u.IsDeleted == false);
			var userDTO = await _mapper.ProjectTo<UserDTO>(query).SingleOrDefaultAsync();

			if (userDTO == null) return NotFound();

			return userDTO;
		}


		[HttpPost]
		public async Task<ActionResult<Guid>> AddUser([FromBody]UserDTO userDTO)
		{
			if (!ModelState.IsValid) return BadRequest();

			var user = _mapper.Map<User>(userDTO);

			await _context.Users.AddAsync(user);
			await _context.SaveChangesAsync();

			return StatusCode(201, user.Id);
		}

		[HttpPut]//("{id}")
		public async Task<IActionResult> UpdateUser([FromBody]UserDTO userDTO, [FromQuery] Guid userId)
		{
			if (!ModelState.IsValid) return BadRequest();

			User user = await _context.Users.SingleOrDefaultAsync(u => u.Id == userId);

			if (_objecState.UserNotFound(user)) return NotFound();

			_mapper.Map(userDTO, user);
			_context.Update(user);
			await _context.SaveChangesAsync();

			return Ok(user.Id);
		}

		// DELETE: api/Users/5
		[HttpDelete]
		public async Task<ActionResult> DeleteUser([FromQuery]Guid userId)
		{
			if (!ModelState.IsValid) return BadRequest();

			var user = await _context.Users.Include(an => an.Announcements).SingleOrDefaultAsync(u => u.Id == userId);

			if (_objecState.UserNotFound(user)) return NotFound();

			user.IsDeleted = true;
			foreach (var announcement in user.Announcements)
			{
				announcement.IsDeleted = true;
			}

			await _context.SaveChangesAsync();

			return StatusCode(204);
		}


	}
}
