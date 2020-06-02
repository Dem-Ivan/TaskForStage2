using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplicationAPI15_SecondStageTS_.Context;
using WebApplicationAPI15_SecondStageTS_.dto;
using WebApplicationAPI15_SecondStageTS_.Models;
using WebApplicationAPI15_SecondStageTS_.utils;
using WebApplicationAPI15_SecondStageTS_.utils.Paging;

namespace WebApplicationAPI15_SecondStageTS_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly IMapper _mapper;

        public UsersController(ApplicationContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<GetResult<UserDTO>>> GetUsers([FromQuery] QueryData queryData, int page = 1, int pageSize = 25)
        {
            try
            {
                IQueryable<User> users = _context.Users;                
                users = users.Where(u => u.IsDeleted == false).GetSortBy(u => u.Name, queryData.sortDirection);//при необходимости можно расширить на сортирову по дополнительным полям
                var pagedResult = users.GetPaged(page, pageSize);
                
                GetResult <UserDTO> result = new GetResult<UserDTO>();
                result.Rows = pagedResult.Result.MappyngTo<UserDTO>(_mapper);
                result.CountRowsFound = pagedResult.RowCount;

                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(new { e.Message, e.StackTrace });
            }
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetUser(Guid id)
        {
            try
            {
                IQueryable<User> query = _context.Users.Where(u => u.Id == id && u.IsDeleted == false);
                var userDTO = await _mapper.ProjectTo<UserDTO>(query).SingleOrDefaultAsync();

                if (userDTO == null)
                {
                    return NotFound();
                }

                return userDTO;
            }
            catch (Exception e)
            {
                return BadRequest(new { e.Message, e.StackTrace });
            }
        }     
       
               
        [HttpPost]
        public async Task<ActionResult<Guid>> PostUser([FromBody]UserDTO userDTO)
        {
            if (!ModelState.IsValid) return BadRequest();

            var user = _mapper.Map<User>(userDTO);

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return StatusCode(201, user.Id); 
        }

        [HttpPut]//("{id}")
        public async Task<IActionResult> PutUser([FromBody]UserDTO userDTO, [FromQuery] Guid userId)
        {
            try
            {              
                User user = await _context.Users.SingleOrDefaultAsync(u => u.Id == userId);

                if (user == null || user.IsDeleted == true) return NotFound();

                _mapper.Map(userDTO, user);                
                _context.Update(user);
                await _context.SaveChangesAsync();

                return Ok(user.Id);
            }
            catch (DbUpdateConcurrencyException e)
            {
                return BadRequest(new { e.Message, e.StackTrace });
            }
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(Guid id)
        {
            var user = await _context.Users.Include(an=>an.Announcements).SingleOrDefaultAsync(u => u.Id == id);
            if (user == null || user.IsDeleted == true) return NotFound();
                     
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
