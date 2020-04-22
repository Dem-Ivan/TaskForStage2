using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplicationAPI15_SecondStageTS_.Context;
using WebApplicationAPI15_SecondStageTS_.dto;
using WebApplicationAPI15_SecondStageTS_.Models;
using WebApplicationAPI15_SecondStageTS_.Services;
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
        public async Task<ActionResult<GetResult<UserDTO>>> GetUsers(int page, int pageSize)
        {
            try
            {
                IQueryable<User> users = _context.Users;
                PagedResult<User> query = users.GetPaged(page, pageSize);      
                GetResult<UserDTO> result = new GetResult<UserDTO>();

                result.CountRowsFound = query.RowCount;
                result.Rows = await _mapper.ProjectTo<UserDTO>(query.Results).ToListAsync();

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
                IQueryable<User> query = _context.Users.Where(u => u.Id == id);
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
        public async Task<ActionResult<Guid>> PostUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(user.Id);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(UserDTO userDTO)
        {
            try
            {
                User user = await _context.Users.SingleOrDefaultAsync(u => u.Id == userDTO.Id);

                if (user == null)
                {
                    return NotFound();
                }

                user.Name = userDTO.Name;
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
        public async Task<ActionResult<User>> DeleteUser(Guid id)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok();
        }

       
    }
}
