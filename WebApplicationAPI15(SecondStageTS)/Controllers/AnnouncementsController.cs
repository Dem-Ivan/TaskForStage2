using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;


using WebApplicationAPI15_SecondStageTS_.Context;
using WebApplicationAPI15_SecondStageTS_.dto;
using WebApplicationAPI15_SecondStageTS_.Models;
using WebApplicationAPI15_SecondStageTS_.Options;
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
       
        public AnnouncementsController(ApplicationContext context, IMapper mapper, IRecaptchaService recaptcha, IOptions<UserOptions> userOptions)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));           
            _recaptcha = recaptcha ?? throw new ArgumentNullException(nameof(recaptcha));          
            _userOptions = userOptions ?? throw new ArgumentNullException(nameof(userOptions));
        }
               
       
        //GET api/announcements/1/5   
        [HttpGet("{page}/{pageSize}")]
        public async Task<ActionResult<GetResult<AnnouncementDTOtoFront>>> GetAnnouncements([FromQuery] QueryData  queryData, int page = 1, int pageSize = 25)
        {
            try
            {
                IQueryable<Announcement> announcements = _context.Announcements.Where(an => an.IsDeleted == false);               
                if (!string.IsNullOrEmpty(queryData.searchString))
                {
                    queryData.searchString = queryData.searchString.ToUpper();
                    announcements = _context.Announcements.Where(s => (EF.Functions.Like(s.Text.ToUpper(), $"%{queryData.searchString}%")) ||
                    (EF.Functions.Like(s.user.Name.ToUpper(), $"%{queryData.searchString}%")) || (EF.Functions.Like(s.OrderNumber.ToString(), $"%{queryData.searchString}%")) ||
                    (EF.Functions.Like(s.Rating.ToString(), $"%{queryData.searchString}%")) || (EF.Functions.Like(s.CreationDate.ToString(), $"%{queryData.searchString}%")) ||
                    (EF.Functions.Like(s.Image.ToUpper(), $"%{queryData.searchString}%")));
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
            catch (ArgumentNullException e)
            {
                return BadRequest(new { e.Message, e.StackTrace });
            }
            catch (Exception e)
            {
                return BadRequest(new { e.Message, e.StackTrace });
            }
        }    
               
       
        //GET api/announcements/5     
        [HttpGet]//("{ id}")
        public async Task<ActionResult<AnnouncementDTOtoFront>> GetAnnouncement(Guid id)
        {            
            try
            {
                var query = _context.Announcements.Where(an => an.Id == id && an.IsDeleted == false);
                var announcementDTO = await _mapper.ProjectTo<AnnouncementDTOtoFront>(query).SingleOrDefaultAsync();

                if (announcementDTO == null)
                {
                    return NotFound();
                }

                return Ok(announcementDTO);
            }
            catch (Exception e)
            {
                return BadRequest(new { e.Message, e.StackTrace });
            }
        }

        //POST api/announcements
        [HttpPost()]       
        public async Task<ActionResult<Guid>> AddAnnouncement([FromBody]AnnouncementDTOtoBack announcementDTO, [FromQuery]Guid userId)
        {
            
            using (var transaction = _context.Database.BeginTransaction( System.Data.IsolationLevel.Serializable))
            {
                try
                {
                    //var captchaResponse = await _recaptcha.Validate(Request.Form);
                    //if (!captchaResponse.Success)
                    //{
                    //    ModelState.AddModelError("reCaptchaError", "reCAPTCHA error occured. Please try again.");
                    //    return RedirectToAction(nameof(Index));
                    //}
                    //if (!ModelState.IsValid) return BadRequest();

                    var user = await _context.Users.Where(x => x.Id == userId).Include(an =>an.Announcements).FirstOrDefaultAsync();

                    if (user == null || user.IsDeleted == true) return NotFound("User not found");

                    if (user.Announcements.Count >= _userOptions.Value.MaxAnnouncementCount)
                    {
                        throw new Exception($"Превышено максимальное колличество объяалений!");
                    }
                    var announcement = _mapper.Map<Announcement>(announcementDTO);
                    announcement.user = user;           
                   
                    _context.Announcements.Add(announcement);
                    await _context.SaveChangesAsync();

                    transaction.Commit();
                    return StatusCode(201, announcement.Id);
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    return BadRequest(new { e.Message, e.StackTrace });
                }
            }
            
        }
       
        //PUT api/announcements
        [HttpPut]
        public async Task<ActionResult<Guid>> UpdateAnnouncement([FromBody]AnnouncementDTOtoBack announcementDTO, [FromQuery]Guid announcementId)
        {
            try
            {
                if (announcementDTO == null) return BadRequest();              
               
                Announcement announcement = await _context.Announcements.Include(u => u.user).SingleOrDefaultAsync(an => an.Id == announcementId);

                if (announcement == null || announcement.IsDeleted == true) return NotFound();
              
                _mapper.Map(announcementDTO, announcement);
                _context.Update(announcement);
                await _context.SaveChangesAsync();

                return Ok(announcement.Id);
            }
            catch (Exception e)
            {
                return BadRequest(new { e.Message, e.StackTrace });
            }
        }

        //DELETE api/announcements/5
        [HttpDelete()]
        public async Task<ActionResult> DeleteAnnouncement([FromQuery]Guid id)
        {
            try
            {
                Announcement announcement = await _context.Announcements.SingleOrDefaultAsync(an => an.Id == id);

                if (announcement == null || announcement.IsDeleted == true) return NotFound();
               
                announcement.IsDeleted = true;               
                await _context.SaveChangesAsync();
                
                return StatusCode(204);
            }
            catch (Exception e)
            {
                return BadRequest(new { e.Message, e.StackTrace });
            }
        }

   
    }
}